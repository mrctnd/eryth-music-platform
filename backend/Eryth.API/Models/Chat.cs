using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Eryth.API.Models
{
    public class Chat
    {
        [Key]
        public Guid ChatId { get; set; } = Guid.NewGuid(); // UUID maps to Guid

        public ChatType ChatType { get; set; } = ChatType.OneToOne;

        public long? LastMessageId { get; set; } // Foreign Key (nullable)
        public virtual Message? LastMessage { get; set; } // Navigation Property

        public DateTime? LastMessageAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
        public virtual ICollection<ChatParticipant> Participants { get; set; } = new List<ChatParticipant>();
    }
}