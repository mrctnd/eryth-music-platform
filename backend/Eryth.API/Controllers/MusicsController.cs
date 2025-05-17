using Eryth.API.DTOs;
using Eryth.API.Services; // IMusicService için
using Microsoft.AspNetCore.Authorization; // [Authorize] için
using Microsoft.AspNetCore.Http; // IFormFile, FromForm için
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; // User.FindFirstValue için
using System.Threading.Tasks;
using System.Collections.Generic; // IEnumerable için

namespace Eryth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // URL: /api/musics
    public class MusicsController : ControllerBase
    {
        private readonly IMusicService _musicService;

        public MusicsController(IMusicService musicService)
        {
            _musicService = musicService;
        }

        // POST /api/musics/upload
        [HttpPost("upload")]
        [Authorize] // Sadece giriş yapmış kullanıcılar müzik yükleyebilir
        [Consumes("multipart/form-data")] // Dosya yükleme için önemli
        public async Task<IActionResult> UploadMusic([FromForm] MusicCreateDto musicCreateDto)
        {
            if (musicCreateDto.MusicFile == null || musicCreateDto.MusicFile.Length == 0)
            {
                ModelState.AddModelError("MusicFile", "Müzik dosyası gereklidir.");
            }
            // Diğer özel validasyonlar burada yapılabilir.
            // Örneğin, dosya tipi, boyutu vb. kontrol edilebilir.

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var uploaderUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(uploaderUserIdString) || !long.TryParse(uploaderUserIdString, out long uploaderUserId))
            {
                return Unauthorized(new { message = "Geçerli bir kullanıcı kimliği bulunamadı." });
            }

            var (succeeded, errorMessage, createdMusic) = await _musicService.CreateMusicAsync(musicCreateDto, uploaderUserId);

            if (succeeded && createdMusic != null)
            {
                // Oluşturulan kaynağın detaylarını getiren bir endpoint'e yönlendirme yapabiliriz:
                // return CreatedAtAction(nameof(GetMusicById), new { musicId = createdMusic.MusicId }, createdMusic);
                // Şimdilik direkt sonucu dönelim:
                return Ok(createdMusic); // Veya 201 Created
            }

            return BadRequest(new { message = errorMessage ?? "Müzik yüklenirken bir hata oluştu." });
        }

        // GET /api/musics/{musicId}
        [HttpGet("{musicId}")]
        public async Task<IActionResult> GetMusicById(long musicId)
        {
            var music = await _musicService.GetMusicByIdAsync(musicId);
            if (music == null)
            {
                return NotFound(new { message = "Müzik bulunamadı." });
            }
            return Ok(music);
        }

        // GET /api/musics/recent
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentMusics([FromQuery] int count = 20)
        {
            var musics = await _musicService.GetRecentlyUploadedMusicsAsync(count);
            return Ok(musics);
        }
    }
}