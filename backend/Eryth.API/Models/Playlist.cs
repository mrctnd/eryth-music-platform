using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Eryth.API.Models
{
    public class Playlist
    {
        [Key]
        public long PlaylistId { get; set; }

        public long CreatorUserId { get; set; } // Foreign Key
        public virtual User CreatorUser { get; set; } = null!; // Navigation Property

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
        public PlaylistVisibility Visibility { get; set; } = PlaylistVisibility.Public;
        public PlaylistStatus Status { get; set; } = PlaylistStatus.Active;
        public bool CanSubscribersAddTracks { get; set; } = false;
        public int TotalDurationSeconds { get; set; } = 0;
        public int TrackCount { get; set; } = 0;
        public long FollowerCount { get; set; } = 0; // Number of saves
        public long LikeCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual ICollection<PlaylistMusic> PlaylistMusics { get; set; } = new List<PlaylistMusic>();
        public virtual ICollection<PlaylistCollaborator> Collaborators { get; set; } = new List<PlaylistCollaborator>();
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Save> Saves { get; set; } = new List<Save>();
    }
}