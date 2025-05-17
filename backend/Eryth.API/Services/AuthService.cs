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

            var authResponse = new AuthResponseDto
            {
                UserId = user.UserId.ToString(),
                Username = user.Username,
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = tokenString,
                TokenExpiration = expiration
            };

            // Opsiyonel: LastLoginDate güncellemesi
            user.LastLoginDate = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

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