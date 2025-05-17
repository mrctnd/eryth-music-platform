using Microsoft.AspNetCore.Http; // IFormFile için
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // List<T> için
using Eryth.API.Models; // DomainEnums.cs dosyasındaki ENUM'lar için

namespace Eryth.API.DTOs
{
    public class MusicCreateDto
    {
        [Required(ErrorMessage = "Müzik başlığı gereklidir.")]
        [StringLength(255, MinimumLength = 1)]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Sanatçı adı gereklidir.")]
        [StringLength(255, MinimumLength = 1)]
        public string PrimaryArtistDisplayText { get; set; } = null!;

        public List<string>? FeaturedArtistsDisplayText { get; set; }

        public long? GenreId { get; set; } // Tür ID'si
        public long? SubGenreId { get; set; } // Alt Tür ID'si
        public List<long>? MoodIds { get; set; } // Mod ID'leri listesi

        public List<string>? Tags { get; set; } // Kullanıcı tanımlı serbest metin etiketler

        [Required(ErrorMessage = "Müzik dosyası gereklidir.")]
        public IFormFile MusicFile { get; set; } = null!; // Yüklenecek müzik dosyası

        public IFormFile? CoverImageFile { get; set; } // Opsiyonel kapak resmi dosyası

        public string? Description { get; set; }
        public string? Lyrics { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public MusicVisibility Visibility { get; set; } = MusicVisibility.Public;
        public bool IsDownloadable { get; set; } = false;

        [Range(1, 7200, ErrorMessage = "Süre 1 ile 7200 saniye arasında olmalıdır.")] // Örnek: max 2 saat
        public int? DurationSeconds { get; set; } // Mümkünse istemciden alınır, değilse sunucuda hesaplanır

        [StringLength(20)]
        public string? Isrc { get; set; }
        public int? Bpm { get; set; }
        [StringLength(10)]
        public string? KeySignature { get; set; }
    }
}