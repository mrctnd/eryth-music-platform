using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Eryth.API.Models
{
    public class Genre
    {
        [Key]
        public long GenreId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Slug { get; set; } = null!;

        public long? ParentGenreId { get; set; } // Nullable FK
        public virtual Genre? ParentGenre { get; set; } // Navigation property for parent

        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }

        // Navigation Properties
        public virtual ICollection<Genre> SubGenres { get; set; } = new List<Genre>(); // Children
        public virtual ICollection<Music> Musics { get; set; } = new List<Music>(); // Bu türe ait müzikler
        public virtual ICollection<Music> SubGenreMusics { get; set; } = new List<Music>(); // Bu alt türe ait müzikler
        public virtual ICollection<Album> Albums { get; set; } = new List<Album>(); // Bu türe ait albümler
    }
}