using System;
using System.Collections.Generic;
using Eryth.API.Models; // DomainEnums.cs dosyasındaki ENUM'lar için

namespace Eryth.API.DTOs
{
    public class MusicViewDto
    {
        public long MusicId { get; set; }
        public string Title { get; set; } = null!;
        public string PrimaryArtistDisplayText { get; set; } = null!;
        public List<string>? FeaturedArtistsDisplayText { get; set; }
        public string? AlbumTitle { get; set; } // Albüm başlığı (varsa)
        public long? AlbumId { get; set; } // Albüm ID'si (varsa)
        public string? GenreName { get; set; } // Tür adı
        public string? SubGenreName { get; set; } // Alt tür adı
        public List<string>? MoodNames { get; set; } // Mod adları
        public List<string>? Tags { get; set; }
        public int DurationSeconds { get; set; }
        public string AudioFileUrl { get; set; } = null!; // Müziğin dinleneceği URL
        public string? WaveformDataUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Description { get; set; }
        public string? Lyrics { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public DateTime UploadDate { get; set; }
        public MusicVisibility Visibility { get; set; }
        public bool IsDownloadable { get; set; }
        public long PlayCount { get; set; }
        public long LikeCount { get; set; }
        public long CommentCount { get; set; }
        public UserSummaryDto Uploader { get; set; } = null!; // Yükleyen kullanıcı bilgisi
    }
}