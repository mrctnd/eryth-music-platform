using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Eryth.API.Models
{
    public class Mood
    {
        [Key]
        public long MoodId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Slug { get; set; } = null!;

        public string? CoverImageUrl { get; set; }

        // Navigation Properties
        public virtual ICollection<MusicMood> MusicMoods { get; set; } = new List<MusicMood>();
    }
}