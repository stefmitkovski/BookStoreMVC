using System.ComponentModel.DataAnnotations;

namespace BookStoreMVC.Models
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        [Display(Name = "Genre Name")]
        public string GenreName { get; set; }

        public ICollection<BookGenre>? bookGenres { get; set; }
    }
}
