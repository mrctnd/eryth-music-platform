// Eryth.API/Services/MusicService.cs
using Eryth.API.Data;
using Eryth.API.DTOs;
using Eryth.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // IConfiguration için eklendi
using System;
using System.Collections.Generic;
using System.IO; // Path, FileStream vb. için
using System.Linq;
using System.Threading.Tasks;

namespace Eryth.API.Services
{
    public class MusicService : IMusicService
    {
        private readonly ErythDbContext _context;
       //  private readonly IConfiguration _configuration;
        private readonly IFileStorageService _fileStorageService; // YENİ
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MusicService(ErythDbContext context, IWebHostEnvironment webHostEnvironment, /*IConfiguration configuration,*/ IFileStorageService fileStorageService)
        {
            _context = context;
            // _configuration = configuration;
            _fileStorageService = fileStorageService; // YENİ
            _webHostEnvironment = webHostEnvironment; // YENİDEN EKLENDİ
        }

        public async Task<(bool Succeeded, string? ErrorMessage, MusicViewDto? Music)> CreateMusicAsync(MusicCreateDto musicCreateDto, long uploaderUserId)
        {
            var uploader = await _context.Users.FindAsync(uploaderUserId);
            if (uploader == null)
            {
                return (false, "Müziği yükleyen kullanıcı bulunamadı.", null);
            }

            if (musicCreateDto.MusicFile == null || musicCreateDto.MusicFile.Length == 0)
            {
                return (false, "Müzik dosyası boş olamaz.", null);
            }

            var musicFileRelativeUrl = await _fileStorageService.SaveFileAsync(musicCreateDto.MusicFile, "music_files");
            if (string.IsNullOrEmpty(musicFileRelativeUrl))
            {
                return (false, "Müzik dosyası kaydedilemedi.", null);
            }

            // --- Müzik Dosyasından Metadata Çıkarma ---
            int durationFromMetadata = 0;
            string? determinedAudioFormat = Path.GetExtension(musicCreateDto.MusicFile.FileName)?.TrimStart('.').ToUpperInvariant();
            int? bitrateFromMetadata = null;

            // Kaydedilen dosyanın tam yolunu almamız gerekiyor TagLib için
            // IFileStorageService'in SaveFileAsync metodu şu anda sadece göreli URL dönüyor.
            // TagLib'in dosyayı okuyabilmesi için tam dosya yoluna ihtiyacı var.
            // Bu yüzden, SaveFileAsync'in ya tam yolu da dönmesini sağlamalıyız ya da
            // burada geçici bir kopya üzerinde çalışmalıyız veya IFileStorageService'e
            // dosyayı okuyup metadata alacak bir metot eklemeliyiz.
            // ŞİMDİLİK BASİT YAKLAŞIM: MusicFile stream'ini kullanarak veya geçici kaydederek okuma.
            // İdealde, IFileStorageService.SaveFileAsync hem göreli URL'i hem de geçici tam yolu dönebilir
            // ya da IFormFile stream'ini doğrudan TagLib'e verebiliriz.

            // IFormFile'dan stream alarak TagLib'e verme (daha verimli olabilir):
            try
            {
                await using var stream = musicCreateDto.MusicFile.OpenReadStream();
                // TagLib, stream'den okuma yaparken dosya adına veya uzantısına ihtiyaç duyabilir formatı belirlemek için.
                // MemoryStream'e kopyalayıp File.Create(stream, "filename.ext", ReadStyle.Average) şeklinde kullanılabilir.
                // VEYA dosyayı geçici bir yere kaydedip oradan okuyabiliriz.
                // SaveFileAsync zaten dosyayı kaydetti. O dosyanın tam yolunu alıp okuyalım.
                
                var webRootPath = _fileStorageService is LocalFileStorageService localFs ? 
                                  (typeof(LocalFileStorageService).GetField("_webHostEnvironment", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(localFs) as IWebHostEnvironment)?.WebRootPath 
                                  : null; // Bu çok ideal bir yol değil, IFileStorageService'in tam yolu vermesi daha iyi.
                                         // VEYA IWebHostEnvironment'ı MusicService'e de enjekte edebiliriz.
                                         // Şimdilik MusicService'e IWebHostEnvironment'ı enjekte edelim.

                // *** MusicService constructor'ına IWebHostEnvironment eklenmeli ***
                // public MusicService(..., IWebHostEnvironment webHostEnvironment)
                // this._webHostEnvironment = webHostEnvironment;
                // (Bir önceki cevabımda IWebHostEnvironment'ı MusicService'ten kaldırmıştım, geri ekleyelim.)

                if (string.IsNullOrEmpty(webRootPath)) { // Fallback to ContentRootPath if WebRootPath is not available or not set (e.g. in tests)
                     webRootPath = (_fileStorageService is LocalFileStorageService localFsService ? 
                                   (typeof(LocalFileStorageService).GetField("_webHostEnvironment", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(localFsService) as IWebHostEnvironment)?.ContentRootPath 
                                   : null) ?? Directory.GetCurrentDirectory(); // Default to current if still null
                }

                string savedMusicFileFullPath = Path.Combine(webRootPath, musicFileRelativeUrl.TrimStart('/'));

                if (File.Exists(savedMusicFileFullPath))
                {
                    var tfile = TagLib.File.Create(savedMusicFileFullPath);
                    durationFromMetadata = (int)tfile.Properties.Duration.TotalSeconds;
                    bitrateFromMetadata = tfile.Properties.AudioBitrate;
                    // determinedAudioFormat = tfile.MimeType; // Daha detaylı format bilgisi alınabilir.
                    tfile.Dispose(); // Dosyayı serbest bırak
                }
                else
                {
                     Console.WriteLine($"Uyarı: Metadata okunacak dosya bulunamadı: {savedMusicFileFullPath}. Süre DTO'dan alınacak veya 0 olacak.");
                     durationFromMetadata = musicCreateDto.DurationSeconds ?? 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Metadata okunurken hata: {ex.Message}. Süre DTO'dan alınacak veya 0 olacak.");
                // Eğer DurationSeconds DTO'dan geliyorsa onu kullan, yoksa 0
                durationFromMetadata = musicCreateDto.DurationSeconds ?? 0;
            }
            // --- Metadata Çıkarma Sonu ---


            string? coverImageRelativeUrl = null;
            if (musicCreateDto.CoverImageFile != null && musicCreateDto.CoverImageFile.Length > 0)
            {
                coverImageRelativeUrl = await _fileStorageService.SaveFileAsync(musicCreateDto.CoverImageFile, "cover_images");
                if (string.IsNullOrEmpty(coverImageRelativeUrl))
                {
                    Console.WriteLine("Uyarı: Kapak resmi kaydedilemedi ancak müzik yükleme işlemine devam ediliyor.");
                }
            }
            
            var music = new Music
            {
                UploaderUserId = uploaderUserId,
                Title = musicCreateDto.Title,
                PrimaryArtistDisplayText = musicCreateDto.PrimaryArtistDisplayText,
                FeaturedArtistsDisplayText = musicCreateDto.FeaturedArtistsDisplayText,
                GenreId = musicCreateDto.GenreId,
                SubGenreId = musicCreateDto.SubGenreId,
                Tags = musicCreateDto.Tags,
                DurationSeconds = durationFromMetadata > 0 ? durationFromMetadata : (musicCreateDto.DurationSeconds ?? 0), // Öncelik metadatadan gelen süre
                AudioFilePath = musicFileRelativeUrl,
                CoverImageUrl = coverImageRelativeUrl,
                Description = musicCreateDto.Description,
                Lyrics = musicCreateDto.Lyrics,
                ReleaseDate = musicCreateDto.ReleaseDate,
                UploadDate = DateTime.UtcNow,
                Visibility = musicCreateDto.Visibility,
                Status = MusicStatus.Active, 
                IsDownloadable = musicCreateDto.IsDownloadable,
                Isrc = musicCreateDto.Isrc,
                Bpm = musicCreateDto.Bpm,
                KeySignature = musicCreateDto.KeySignature,
                AudioFileFormat = determinedAudioFormat,
                AudioBitrateKbitps = bitrateFromMetadata,
                // AudioFileSizeMB = musicCreateDto.MusicFile.Length / (1024.0 * 1024.0) // Dosya boyutu MB cinsinden
                AudioFileSizeMb = (decimal?)Math.Round(musicCreateDto.MusicFile.Length / (1024.0 * 1024.0), 2),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // ... (MusicMoods ve veritabanına kaydetme kısımları aynı) ...
            if (musicCreateDto.MoodIds != null && musicCreateDto.MoodIds.Any())
            {
                foreach (var moodId in musicCreateDto.MoodIds)
                {
                    music.MusicMoods.Add(new MusicMood { MoodId = moodId });
                }
            }

            try
            {
                await _context.Musics.AddAsync(music);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                await _fileStorageService.DeleteFileAsync(musicFileRelativeUrl);
                if (!string.IsNullOrEmpty(coverImageRelativeUrl))
                {
                    await _fileStorageService.DeleteFileAsync(coverImageRelativeUrl);
                }
                return (false, $"Veritabanına kayıt sırasında bir hata oluştu: {ex.InnerException?.Message ?? ex.Message}", null);
            }
            
            // MusicViewDto oluşturma...
            var genre = music.GenreId.HasValue ? await _context.Genres.FindAsync(music.GenreId.Value) : null;
            var uploaderInfo = new UserSummaryDto {
                UserId = uploader.UserId, Username = uploader.Username, DisplayName = uploader.DisplayName, ProfilePictureUrl = uploader.ProfilePictureUrl
            };
            var musicView = new MusicViewDto { 
                MusicId = music.MusicId, Title = music.Title, PrimaryArtistDisplayText = music.PrimaryArtistDisplayText,
                FeaturedArtistsDisplayText = music.FeaturedArtistsDisplayText, GenreName = genre?.Name,
                DurationSeconds = music.DurationSeconds, AudioFileUrl = music.AudioFilePath, CoverImageUrl = music.CoverImageUrl,   
                UploadDate = music.UploadDate, Visibility = music.Visibility, PlayCount = music.PlayCount, LikeCount = music.LikeCount,
                Uploader = uploaderInfo,
                // Diğer alanlar (SubGenreName, MoodNames, Lyrics, Description vb. GetMusicByIdAsync'te daha detaylı doldurulabilir)
                IsDownloadable = music.IsDownloadable,
                ReleaseDate = music.ReleaseDate
            };
            return (true, null, musicView);
        }

        // ... (GetMusicByIdAsync ve GetRecentlyUploadedMusicsAsync iskeletleri) ...
        public async Task<MusicViewDto?> GetMusicByIdAsync(long musicId)
        {
            var music = await _context.Musics
                .Include(m => m.UploaderUser) // Yükleyen kullanıcı bilgisini çek
                .Include(m => m.Genre)        // Tür bilgisini çek
                .Include(m => m.SubGenre)     // Alt tür bilgisini çek
                .Include(m => m.MusicMoods)   // Müzik-Mod ilişkilerini çek
                    .ThenInclude(mm => mm.Mood) // Mod bilgilerini çek
                .FirstOrDefaultAsync(m => m.MusicId == musicId && m.Status == MusicStatus.Active); // Sadece aktif müzikler

            if (music == null) return null;

            return new MusicViewDto
            {
                MusicId = music.MusicId,
                Title = music.Title,
                PrimaryArtistDisplayText = music.PrimaryArtistDisplayText,
                FeaturedArtistsDisplayText = music.FeaturedArtistsDisplayText,
                AlbumTitle = music.AlbumMusics.FirstOrDefault()?.Album.Title, // Bu daha karmaşık olabilir, şimdilik basit tutalım
                AlbumId = music.AlbumMusics.FirstOrDefault()?.AlbumId,
                GenreName = music.Genre?.Name,
                SubGenreName = music.SubGenre?.Name,
                MoodNames = music.MusicMoods.Select(mm => mm.Mood.Name).ToList(),
                Tags = music.Tags,
                DurationSeconds = music.DurationSeconds,
                AudioFileUrl = music.AudioFilePath,
                WaveformDataUrl = music.WaveformDataPath,
                CoverImageUrl = music.CoverImageUrl,
                Description = music.Description,
                Lyrics = music.Lyrics,
                ReleaseDate = music.ReleaseDate,
                UploadDate = music.UploadDate,
                Visibility = music.Visibility,
                IsDownloadable = music.IsDownloadable,
                PlayCount = music.PlayCount,
                LikeCount = music.LikeCount,
                CommentCount = music.CommentCount,
                Uploader = new UserSummaryDto
                {
                    UserId = music.UploaderUser.UserId,
                    Username = music.UploaderUser.Username,
                    DisplayName = music.UploaderUser.DisplayName,
                    ProfilePictureUrl = music.UploaderUser.ProfilePictureUrl
                }
            };
        }

        public async Task<IEnumerable<MusicViewDto>> GetRecentlyUploadedMusicsAsync(int count = 20)
        {
            var musics = await _context.Musics
                .Where(m => m.Status == MusicStatus.Active && m.Visibility == MusicVisibility.Public) // Sadece aktif ve public
                .OrderByDescending(m => m.UploadDate)
                .Take(count)
                .Include(m => m.UploaderUser)
                .Include(m => m.Genre)
                .Select(music => new MusicViewDto // Projeksiyon ile sadece gerekli alanları çek
                {
                    MusicId = music.MusicId,
                    Title = music.Title,
                    PrimaryArtistDisplayText = music.PrimaryArtistDisplayText,
                    CoverImageUrl = music.CoverImageUrl,
                    AudioFileUrl = music.AudioFilePath,
                    DurationSeconds = music.DurationSeconds,
                    UploadDate = music.UploadDate,
                    Uploader = new UserSummaryDto
                    {
                        UserId = music.UploaderUser.UserId,
                        Username = music.UploaderUser.Username,
                        DisplayName = music.UploaderUser.DisplayName,
                        ProfilePictureUrl = music.UploaderUser.ProfilePictureUrl
                    },
                    GenreName = music.Genre != null ? music.Genre.Name : null,
                    PlayCount = music.PlayCount,
                    LikeCount = music.LikeCount
                })
                .ToListAsync();

            return musics;
        }
    }
}