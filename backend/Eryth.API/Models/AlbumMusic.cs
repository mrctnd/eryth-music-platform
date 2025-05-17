using System.ComponentModel.DataAnnotations;

namespace Eryth.API.Models
{
    public class AlbumMusic // Junction table for Album and Music (Many-to-Many)
    {
        [Key]
        public long AlbumMusicId { get; set; }

        public long AlbumId { get; set; } // Foreign Key
        public virtual Album Album { get; set; } = null!; // Navigation Property

        public long MusicId { get; set; } // Foreign Key
        public virtual Music Music { get; set; } = null!; // Navigation Property

        public int DiscNumber { get; set; } = 1;
        public int TrackNumber { get; set; }
    }
}