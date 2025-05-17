using System;
using System.ComponentModel.DataAnnotations;

namespace Eryth.API.Models
{
    public class Notification
    {
        [Key]
        public Guid NotificationId { get; set; } = Guid.NewGuid(); // UUID maps to Guid

        public long ReceiverUserId { get; set; } // Foreign Key
        public virtual User ReceiverUser { get; set; } = null!; // Navigation Property

        public NotificationType NotificationType { get; set; }

        public long? ActorUserId { get; set; } // Foreign Key (nullable)
        public virtual User? ActorUser { get; set; } // Navigation Property

        public PrimaryEntityType? PrimaryEntityType { get; set; }
        public long? PrimaryEntityId { get; set; }

        public string? ContentText { get; set; }
        public string? LinkUrl { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}