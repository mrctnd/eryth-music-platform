using Eryth.API.Data;
using Eryth.API.DTOs;
using Eryth.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // IConfiguration için
using System.IdentityModel.Tokens.Jwt;    // JwtSecurityTokenHandler için
using System.Security.Claims;             // Claims için
using System.Text;                        // Encoding.UTF8 için
using Microsoft.IdentityModel.Tokens;     // SymmetricSecurityKey, SigningCredentials için
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic; // IEnumerable<Claim> için

namespace Eryth.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly ErythDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ErythDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<(bool Succeeded, string? ErrorMessage, User? User)> RegisterUserAsync(UserRegisterDto registerDto)
        {
            // 1. Kullanıcı adı veya e-posta zaten var mı kontrol et
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
            {
                return (false, "Bu kullanıcı adı zaten kullanılıyor.", null);
            }

            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                return (false, "Bu e-posta adresi zaten kullanılıyor.", null);
            }

            // 2. Şifreyi hash'le
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // 3. Yeni User entity'si oluştur
            var user = new User
            {
                Username = registerDto.Username,
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                PasswordHash = hashedPassword,
                Role = UserRole.User, // Varsayılan rol
                Status = UserStatus.Active, // Veya e-posta doğrulaması için PendingVerification
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // 4. Kullanıcıyı veritabanına ekle
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Kullanıcı kayıt aktivitesini logla
            var registerLog = new UserActivityLog
            {
                UserId = user.UserId,
                ActivityType = ActivityType.UserRegistered,
                Timestamp = DateTime.UtcNow
            };
            await _context.UserActivityLogs.AddAsync(registerLog);
            await _context.SaveChangesAsync();

            return (true, null, user);
        }

        public async Task<(bool Succeeded, string? ErrorMessage, AuthResponseDto? AuthResponse)> LoginUserAsync(UserLoginDto loginDto)
        {
            // 1. Kullanıcıyı kullanıcı adı veya e-posta ile bul
            var user = await _context.Users.FirstOrDefaultAsync(u => 
                u.Username == loginDto.LoginCredential || u.Email == loginDto.LoginCredential);

            if (user == null)
            {
                return (false, "Kullanıcı adı/e-posta veya şifre hatalı.", null);
            }

            // 2. Kullanıcının durumu aktif mi kontrol et (opsiyonel)
            if (user.Status != UserStatus.Active)
            {
                 if (user.Status == UserStatus.PendingVerification)
                    return (false, "Hesabınız henüz doğrulanmamış. Lütfen e-postanızı kontrol edin.", null);
                return (false, "Hesabınız aktif değil.", null);
            }

            // 3. Şifreyi doğrula
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return (false, "Kullanıcı adı/e-posta veya şifre hatalı.", null);
            }

            // 4. JWT token üret
            var (tokenString, expiration) = GenerateJwtToken(user);

            // 5. Refresh token üret
            var refreshToken = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
            var refreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken);
            var userSession = new UserSession
            {
                UserId = user.UserId,
                RefreshTokenHash = refreshTokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                LastAccessedAt = DateTime.UtcNow,
                UserAgent = null, // İsterseniz loginDto'dan veya header'dan alabilirsiniz
                IpAddress = null  // İsterseniz loginDto'dan veya header'dan alabilirsiniz
            };
            await _context.UserSessions.AddAsync(userSession);

            // Opsiyonel: LastLoginDate güncellemesi
            user.LastLoginDate = DateTime.UtcNow;
            _context.Users.Update(user);

            // Kullanıcı giriş aktivitesini logla
            var loginLog = new UserActivityLog
            {
                UserId = user.UserId,
                ActivityType = ActivityType.UserLoggedIn,
                Timestamp = DateTime.UtcNow
            };
            await _context.UserActivityLogs.AddAsync(loginLog);

            await _context.SaveChangesAsync();

            var authResponse = new AuthResponseDto
            {
                UserId = user.UserId.ToString(),
                Username = user.Username,
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = tokenString,
                TokenExpiration = expiration,
                RefreshToken = refreshToken
            };

            return (true, null, authResponse);
        }

        public async Task<(bool Succeeded, string? ErrorMessage, AuthResponseDto? AuthResponse)> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenRequestDto.RefreshToken))
                return (false, "Refresh token gerekli.", null);

            // Tüm aktif oturumları çek
            var now = DateTime.UtcNow;
            var userSessions = await _context.UserSessions
                .Include(us => us.User)
                .Where(us => us.ExpiresAt > now)
                .ToListAsync();

            UserSession? matchedSession = null;
            foreach (var session in userSessions)
            {
                if (BCrypt.Net.BCrypt.Verify(refreshTokenRequestDto.RefreshToken, session.RefreshTokenHash))
                {
                    matchedSession = session;
                    break;
                }
            }

            if (matchedSession == null)
                return (false, "Refresh token geçersiz veya süresi dolmuş.", null);

            var user = matchedSession.User;
            if (user == null)
                return (false, "Kullanıcı bulunamadı.", null);

            // Yeni access token üret
            var (tokenString, expiration) = GenerateJwtToken(user);

            // Yeni refresh token üret ve kaydet
            var newRefreshToken = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
            var newRefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(newRefreshToken);
            matchedSession.RefreshTokenHash = newRefreshTokenHash;
            matchedSession.ExpiresAt = DateTime.UtcNow.AddDays(7);
            matchedSession.LastAccessedAt = DateTime.UtcNow;
            _context.UserSessions.Update(matchedSession);
            await _context.SaveChangesAsync();

            var authResponse = new AuthResponseDto
            {
                UserId = user.UserId.ToString(),
                Username = user.Username,
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = tokenString,
                TokenExpiration = expiration,
                RefreshToken = newRefreshToken
            };

            return (true, null, authResponse);
        }

        private (string Token, DateTime Expiration) GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expirationInMinutes = Convert.ToInt32(jwtSettings["ExpirationInMinutes"]);

            if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                throw new ApplicationException("JWT ayarları (SecretKey, Issuer, Audience) appsettings.json dosyasında eksik veya hatalı.");
            }
             if (secretKey.Length < 32)
            {
                throw new ApplicationException("JWT SecretKey en az 32 karakter uzunluğunda olmalıdır.");
            }


            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()), // Subject (kullanıcı ID'si)
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),   // Kullanıcı adı
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("displayName", user.DisplayName), // Özel claim
                new Claim(ClaimTypes.Role, user.Role.ToString()) // Rol
                // İhtiyaç duyulan diğer claim'ler eklenebilir
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            return (tokenHandler.WriteToken(token), tokenDescriptor.Expires.Value);
        }
    }
}