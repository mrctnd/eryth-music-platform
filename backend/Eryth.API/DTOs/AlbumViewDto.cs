using System;
using System.Collections.Generic;
using Eryth.API.Models; // DomainEnums.cs dosyasındaki ENUM'lar için

namespace Eryth.API.DTOs
{
    public class AlbumViewDto
    {
        public long AlbumId { get; set; }
        public string Title { get; set; } = null!;
        public AlbumType AlbumType { get; set; }
        public string PrimaryArtistDisplayText { get; set; } = null!;
        public string? CoverImageUrl { get; set; }
        public string? Description { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? GenreName { get; set; }
        public UserSummaryDto Creator { get; set; } = null!; // Oluşturan kullanıcı
        public List<MusicTrackViewDto> Tracks { get; set; } = new List<MusicTrackViewDto>(); // Albümdeki şarkılar
        public long TotalPlayCount { get; set; }
        public long TotalLikeCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class MusicTrackViewDto // AlbumViewDto içinde kullanılacak
    {
        public long MusicId { get; set; }
        public string Title { get; set; } = null!;
        public int TrackNumber { get; set; }
        public int DiscNumber { get; set; }
        public int DurationSeconds { get; set; }
        public string AudioFileUrl { get; set; } = null!;
        public long PlayCount { get; set; }
    }
}