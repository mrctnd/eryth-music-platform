using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Eryth.API.Models; // DomainEnums.cs dosyasındaki ENUM'lar için

namespace Eryth.API.DTOs
{
    public class AlbumCreateDto
    {
        [Required(ErrorMessage = "Albüm başlığı gereklidir.")]
        [StringLength(255, MinimumLength = 1)]
        public string Title { get; set; } = null!;

        [Required]
        public AlbumType AlbumType { get; set; }

        [Required(ErrorMessage = "Sanatçı adı gereklidir.")]
        [StringLength(255, MinimumLength = 1)]
        public string PrimaryArtistDisplayText { get; set; } = null!;

        public IFormFile? CoverImageFile { get; set; } // Opsiyonel kapak resmi dosyası

        public string? Description { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public long? GenreId { get; set; }
        [StringLength(20)]
        public string? Upc { get; set; }
        [StringLength(100)]
        public string? RecordLabelName { get; set; }
        public AlbumVisibility Visibility { get; set; } = AlbumVisibility.Public;

        // Albüme eklenecek müziklerin ID'leri (opsiyonel, albüm oluşturulurken eklenebilir)
        public List<long>? MusicIds { get; set; }
    }
}