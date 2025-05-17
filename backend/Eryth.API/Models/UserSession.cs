using System;
using System.ComponentModel.DataAnnotations;

namespace Eryth.API.Models
{
    public class UserSession
    {
        [Key]
        public Guid SessionId { get; set; } = Guid.NewGuid(); // UUID maps to Guid

        public long UserId { get; set; } // Foreign Key
        public virtual User User { get; set; } = null!; // Navigation Property

        [Required]
        public string RefreshTokenHash { get; set; } = null!;
        public string? UserAgent { get; set; }

        [StringLength(45)]
        public string? IpAddress { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastAccessedAt { get; set; } = DateTime.UtcNow;
    }
}