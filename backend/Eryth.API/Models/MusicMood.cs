using System.ComponentModel.DataAnnotations;

namespace Eryth.API.Models
{
    public class MusicMood // Junction table for Music and Mood (Many-to-Many)
    {
        [Key]
        public long MusicMoodId { get; set; } // Optional: Or use composite key in DbContext

        public long MusicId { get; set; } // Foreign Key
        public virtual Music Music { get; set; } = null!; // Navigation Property

        public long MoodId { get; set; } // Foreign Key
        public virtual Mood Mood { get; set; } = null!; // Navigation Property
    }
}