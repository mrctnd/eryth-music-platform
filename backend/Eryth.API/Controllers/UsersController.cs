using Microsoft.AspNetCore.Authorization; // [Authorize] attribute'ü için
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; // User.FindFirstValue için

namespace Eryth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        // GET api/users/me
        [HttpGet("me")]
        [Authorize] // Bu endpoint'e erişmek için geçerli bir JWT token gerekir
        public IActionResult GetCurrentUserProfile()
        {
            // Token'dan kullanıcı bilgilerini alabiliriz
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Token'daki "sub" claim'i (UserId)
            var username = User.FindFirstValue(ClaimTypes.Name); // Token'daki "unique_name" claim'i (Username)
            var email = User.FindFirstValue(ClaimTypes.Email); // Token'daki "email" claim'i
            var displayName = User.FindFirstValue("displayName"); // Token'daki özel "displayName" claim'i
            var role = User.FindFirstValue(ClaimTypes.Role); // Token'daki "role" claim'i

            // User.Claims koleksiyonunu da inceleyebilirsiniz:
            // var allClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Kullanıcı kimliği token'da bulunamadı." });
            }

            return Ok(new 
            { 
                UserId = userId, 
                Username = username, 
                Email = email,
                DisplayName = displayName,
                Role = role
                // Claims = allClaims // İsterseniz tüm claim'leri de dönebilirsiniz test için
            });
        }
    }
}