using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Eryth.API.Models
{
    public class UserDefinedTag
    {
        [Key]
        public long TagId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Slug { get; set; } = null!;

        public long UsageCount { get; set; } = 0;

        // Navigation Properties
        public virtual ICollection<MusicUserDefinedTag> MusicUserDefinedTags { get; set; } = new List<MusicUserDefinedTag>();
    }
}