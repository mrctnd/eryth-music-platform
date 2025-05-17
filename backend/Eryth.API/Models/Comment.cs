using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Eryth.API.Models
{
    public class Comment
    {
        [Key]
        public long CommentId { get; set; }

        public long UserId { get; set; } // Foreign Key
        public virtual User User { get; set; } = null!; // Navigation Property

        public CommentedEntityType CommentedEntityType { get; set; }
        public long CommentedEntityId { get; set; } // ID of Music, Album, or Playlist

        public long? ParentCommentId { get; set; } // Foreign Key for replies
        public virtual Comment? ParentComment { get; set; } // Navigation to parent comment

        [Required]
        public string Text { get; set; } = null!;
        public CommentStatus Status { get; set; } = CommentStatus.Active;
        public long LikeCount { get; set; } = 0;

        public DateTime CommentedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>(); // Replies to this comment
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}