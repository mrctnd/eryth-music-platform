using System;
using System.ComponentModel.DataAnnotations;

namespace Eryth.API.Models
{
    public class Save // For saving Albums or Playlists to a user's library
    {
        [Key]
        public long SaveId { get; set; }

        public long UserId { get; set; } // Foreign Key
        public virtual User User { get; set; } = null!; // Navigation Property

        public LikedEntityType SavedEntityType { get; set; } // Should be Album or Playlist
        public long SavedEntityId { get; set; }

        public DateTime SavedAt { get; set; } = DateTime.UtcNow;
    }
}