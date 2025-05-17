using System;
using System.ComponentModel.DataAnnotations;

namespace Eryth.API.Models
{
    public class PlaylistCollaborator
    {
        [Key]
        public long PlaylistCollaboratorId { get; set; }

        public long PlaylistId { get; set; } // Foreign Key
        public virtual Playlist Playlist { get; set; } = null!; // Navigation Property

        public long UserId { get; set; } // Foreign Key
        public virtual User User { get; set; } = null!; // Navigation Property

        public PlaylistCollaboratorRole Role { get; set; } = PlaylistCollaboratorRole.Viewer;
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}