using System;
using System.ComponentModel.DataAnnotations;

namespace Eryth.API.Models
{
    public class Message
    {
        [Key]
        public long MessageId { get; set; }

        public Guid ChatId { get; set; } // Foreign Key
        public virtual Chat Chat { get; set; } = null!; // Navigation Property

        public long SenderUserId { get; set; } // Foreign Key
        public virtual User SenderUser { get; set; } = null!; // Navigation Property

        public MessageType MessageType { get; set; } = MessageType.Text;

        [Required]
        public string ContentText { get; set; } = null!;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public MessageStatus Status { get; set; } = MessageStatus.Sent;
        public bool DeletedForSender { get; set; } = false;
        public bool DeletedForEveryone { get; set; } = false;
    }
}