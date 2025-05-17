using Eryth.API.DTOs;
using Eryth.API.Services; // IAlbumService için
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Eryth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // URL: /api/albums
    public class AlbumsController : ControllerBase
    {
        private readonly IAlbumService _albumService;

        public AlbumsController(IAlbumService albumService)
        {
            _albumService = albumService;
        }

        // POST /api/albums
        [HttpPost]
        [Authorize] // Sadece giriş yapmış kullanıcılar albüm oluşturabilir
        [Consumes("multipart/form-data")] // Kapak resmi yükleme için
        public async Task<IActionResult> CreateAlbum([FromForm] AlbumCreateDto albumCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var creatorUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(creatorUserIdString) || !long.TryParse(creatorUserIdString, out long creatorUserId))
            {
                return Unauthorized(new { message = "Geçerli bir kullanıcı kimliği bulunamadı." });
            }

            var (succeeded, errorMessage, createdAlbum) = await _albumService.CreateAlbumAsync(albumCreateDto, creatorUserId);

            if (succeeded && createdAlbum != null)
            {
                // return CreatedAtAction(nameof(GetAlbumById), new { albumId = createdAlbum.AlbumId }, createdAlbum);
                return Ok(createdAlbum); // Veya 201 Created
            }

            return BadRequest(new { message = errorMessage ?? "Albüm oluşturulurken bir hata oluştu." });
        }

        // GET /api/albums/{albumId}
        [HttpGet("{albumId}")]
        public async Task<IActionResult> GetAlbumById(long albumId)
        {
            var album = await _albumService.GetAlbumByIdAsync(albumId);
            if (album == null)
            {
                return NotFound(new { message = "Albüm bulunamadı." });
            }
            return Ok(album);
        }
    }
}