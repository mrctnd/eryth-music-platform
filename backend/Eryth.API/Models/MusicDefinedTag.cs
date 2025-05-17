using System.ComponentModel.DataAnnotations;

namespace Eryth.API.Models
{
    public class MusicUserDefinedTag // Junction table for Music and UserDefinedTag (Many-to-Many)
    {
        [Key]
        public long MusicTagId { get; set; }

        public long MusicId { get; set; } // Foreign Key
        public virtual Music Music { get; set; } = null!; // Navigation Property

        public long TagId { get; set; } // Foreign Key
        public virtual UserDefinedTag UserDefinedTag { get; set; } = null!; // Navigation Property
    }
}