using System;
using System.ComponentModel.DataAnnotations;

namespace Eryth.API.Models
{
    public class UserAuthenticationProvider
    {
        [Key]
        public long UserAuthProviderId { get; set; }

        public long UserId { get; set; } // Foreign Key
        public virtual User User { get; set; } = null!; // Navigation Property

        [Required]
        [StringLength(50)]
        public string ProviderName { get; set; } = null!;

        [Required]
        public string ProviderUserId { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}