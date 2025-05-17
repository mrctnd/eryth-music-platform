using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eryth.API.Models
{
    public class UserActivityLog
    {
        [Key]
        public long ActivityLogId { get; set; }

        public long? UserId { get; set; } // Foreign Key (nullable)
        public virtual User? User { get; set; } // Navigation Property

        public ActivityType ActivityType { get; set; }
        public PrimaryEntityType? PrimaryEntityType { get; set; }
        public long? PrimaryEntityId { get; set; }

        [Column(TypeName = "jsonb")]
        public string? Details { get; set; } // JSON string

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}