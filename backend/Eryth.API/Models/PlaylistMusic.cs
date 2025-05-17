using System;
using System.ComponentModel.DataAnnotations;

namespace Eryth.API.Models
{
    public class PlaylistMusic // Junction table for Playlist and Music (Many-to-Many)
    {
        [Key]
        public long PlaylistMusicId { get; set; }

        public long PlaylistId { get; set; } // Foreign Key
        public virtual Playlist Playlist { get; set; } = null!; // Navigation Property

        public long MusicId { get; set; } // Foreign Key
        public virtual Music Music { get; set; } = null!; // Navigation Property

        public long AddedByUserId { get; set; } // Foreign Key
        public virtual User AddedByUser { get; set; } = null!; // Navigation Property

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        public int? CustomOrder { get; set; }
    }
}