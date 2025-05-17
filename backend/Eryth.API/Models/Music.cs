using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eryth.API.Models
{
    public class Music
    {
        [Key]
        public long MusicId { get; set; }

        public long UploaderUserId { get; set; } // Foreign Key
        public virtual User UploaderUser { get; set; } = null!; // Navigation Property

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string PrimaryArtistDisplayText { get; set; } = null!;

        public List<string>? FeaturedArtistsDisplayText { get; set; } // TEXT[] maps to List<string> or string[]

        public long? GenreId { get; set; } // Foreign Key
        public virtual Genre? Genre { get; set; } // Navigation Property

        public long? SubGenreId { get; set; } // Foreign Key
        public virtual Genre? SubGenre { get; set; } // Navigation Property for SubGenre

        public List<string>? Tags { get; set; } // TEXT[] maps to List<string> or string[]

        public int DurationSeconds { get; set; }

        [Required]
        public string AudioFilePath { get; set; } = null!;
        public string? WaveformDataPath { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Description { get; set; }
        public string? Lyrics { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
        public MusicVisibility Visibility { get; set; } = MusicVisibility.Public;
        public MusicStatus Status { get; set; } = MusicStatus.Processing;
        public bool IsDownloadable { get; set; } = false;

        [StringLength(20)]
        public string? Isrc { get; set; }
        public int? Bpm { get; set; }

        [StringLength(10)]
        public string? KeySignature { get; set; }

        [StringLength(10)]
        public string? AudioFileFormat { get; set; }
        public int? AudioBitrateKbitps { get; set; }
        public decimal? AudioFileSizeMb { get; set; } // DECIMAL(10,2) maps to decimal

        public long PlayCount { get; set; } = 0;
        public long LikeCount { get; set; } = 0;
        public long CommentCount { get; set; } = 0;
        public long RepostCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual ICollection<MusicMood> MusicMoods { get; set; } = new List<MusicMood>();
        public virtual ICollection<AlbumMusic> AlbumMusics { get; set; } = new List<AlbumMusic>();
        public virtual ICollection<MusicUserDefinedTag> MusicUserDefinedTags { get; set; } = new List<MusicUserDefinedTag>();
        public virtual ICollection<PlaylistMusic> PlaylistMusics { get; set; } = new List<PlaylistMusic>();
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}