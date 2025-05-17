using System;
using System.ComponentModel.DataAnnotations;

namespace Eryth.API.Models
{
    public class ChatParticipant // Junction table for Chat and User (Many-to-Many)
    {
        [Key]
        public long ChatParticipantId { get; set; }

        public Guid ChatId { get; set; } // Foreign Key
        public virtual Chat Chat { get; set; } = null!; // Navigation Property

        public long UserId { get; set; } // Foreign Key
        public virtual User User { get; set; } = null!; // Navigation Property

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public long? LastReadMessageId { get; set; } // Foreign Key (nullable)
        public virtual Message? LastReadMessage { get; set; } // Navigation Property

        public bool NotificationsEnabled { get; set; } = true;
    }
}