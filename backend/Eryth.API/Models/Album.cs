using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eryth.API.Models
{
    public class Album
    {
        [Key]
        public long AlbumId { get; set; }

        public long CreatorUserId { get; set; } // Foreign Key
        public virtual User CreatorUser { get; set; } = null!; // Navigation Property

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = null!;

        public AlbumType AlbumType { get; set; }

        [Required]
        [StringLength(255)]
        public string PrimaryArtistDisplayText { get; set; } = null!;

        public string? CoverImageUrl { get; set; }
        public string? Description { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public long? GenreId { get; set; } // Foreign Key
        public virtual Genre? Genre { get; set; } // Navigation Property

        [StringLength(20)]
        public string? Upc { get; set; }

        [StringLength(100)]
        public string? RecordLabelName { get; set; }

        public AlbumVisibility Visibility { get; set; } = AlbumVisibility.Public;
        public AlbumStatus Status { get; set; } = AlbumStatus.Active;

        public long TotalPlayCount { get; set; } = 0;
        public long TotalLikeCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual ICollection<AlbumMusic> AlbumMusics { get; set; } = new List<AlbumMusic>();
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Save> Saves { get; set; } = new List<Save>();
    }
}