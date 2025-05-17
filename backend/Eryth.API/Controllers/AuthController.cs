using Eryth.API.DTOs;
using Eryth.API.Services; // IAuthService için
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Eryth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // URL: /api/auth
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST /api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Model validasyon hatalarını döner
            }

            var (succeeded, errorMessage, user) = await _authService.RegisterUserAsync(registerDto);

            if (succeeded && user != null)
            {
                // Başarılı kayıt sonrası ne döneceğimize karar vermeliyiz.
                // Genellikle 201 Created status kodu ve oluşturulan kaynağın bir temsili veya konumu döner.
                // Şimdilik basit bir mesaj veya kullanıcı bilgisi dönebiliriz.
                // Ya da doğrudan bir AuthResponseDto (token ile) dönebiliriz eğer kayıt sonrası otomatik giriş isteniyorsa.
                // MVP için: Başarılı mesajı ve belki kullanıcı ID'si.
                return CreatedAtAction(nameof(Register), new { userId = user.UserId }, new { message = "Kullanıcı başarıyla kaydedildi.", userId = user.UserId });
            }

            // ModelState'e özel hata mesajını ekleyebiliriz.
            // Örneğin, "Username already taken" gibi.
            if (!string.IsNullOrEmpty(errorMessage))
            {
                // Genel bir hata mesajı olarak da dönebiliriz veya ModelState'e ekleyebiliriz.
                // ModelState.AddModelError(string.Empty, errorMessage); // Anahtar boşsa genel hata.
                // return BadRequest(ModelState);
                return BadRequest(new { message = errorMessage });
            }

            return BadRequest(new { message = "Kullanıcı kaydı oluşturulamadı." }); // Genel hata
        }

        // POST /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (succeeded, errorMessage, authResponse) = await _authService.LoginUserAsync(loginDto);

            if (succeeded && authResponse != null)
            {
                return Ok(authResponse); // Token ve kullanıcı bilgilerini içeren AuthResponseDto döner
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                 // Güvenlik açısından, "kullanıcı adı veya şifre hatalı" gibi genel bir mesaj dönmek daha iyidir.
                return Unauthorized(new { message = errorMessage }); // 401 Unauthorized
            }

            return Unauthorized(new { message = "Giriş başarısız." }); // Genel yetkisiz erişim hatası
        }
    }
}