using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eryth.API.Models
{
    public class Follow
    {
        [Key]
        public long FollowId { get; set; }

        public long FollowerUserId { get; set; } // User who is following
        public virtual User FollowerUser { get; set; } = null!;

        public long FollowingUserId { get; set; } // User who is being followed
        public virtual User FollowingUser { get; set; } = null!;

        public DateTime FollowedAt { get; set; } = DateTime.UtcNow;
    }
}