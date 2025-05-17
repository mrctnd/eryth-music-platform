using Microsoft.AspNetCore.Authorization; // [Authorize] attribute'ü için
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; // User.FindFirstValue için
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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

        // GET api/users/me/activity-logs
        [HttpGet("me/activity-logs")]
        [Authorize]
        public async Task<IActionResult> GetMyActivityLogs([FromServices] Eryth.API.Data.ErythDbContext dbContext)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !long.TryParse(userIdStr, out var userId))
                return Unauthorized(new { message = "Kullanıcı kimliği token'da bulunamadı." });

            var logs = await dbContext.UserActivityLogs
                .Where(log => log.UserId == userId)
                .OrderByDescending(log => log.Timestamp)
                .Take(50)
                .ToListAsync();

            return Ok(logs);
        }

        // TEST: Global Exception Middleware çalışıyor mu?
        [HttpGet("test-exception")]
        [AllowAnonymous]
        public IActionResult ThrowTestException()
        {
            throw new Exception("Test amaçlı fırlatılan exception.");
        }
    }
}