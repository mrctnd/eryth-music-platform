using System;
using System.ComponentModel.DataAnnotations;

namespace Eryth.API.Models
{
    public class Like
    {
        [Key]
        public long LikeId { get; set; }

        public long UserId { get; set; } // Foreign Key
        public virtual User User { get; set; } = null!; // Navigation Property

        public LikedEntityType LikedEntityType { get; set; }
        public long LikedEntityId { get; set; } // ID of Music, Album, Playlist, or Comment

        // Optional: Navigation properties to specific liked entities (requires careful setup in DbContext if not using TPH/TPT for LikedEntity)
        // public virtual Music? LikedMusic { get; set; }
        // public virtual Album? LikedAlbum { get; set; }
        // public virtual Playlist? LikedPlaylist { get; set; }
        // public virtual Comment? LikedComment { get; set; }

        public DateTime LikedAt { get; set; } = DateTime.UtcNow;
    }
}