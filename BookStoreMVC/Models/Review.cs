using System.ComponentModel.DataAnnotations;

namespace BookStoreMVC.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Book")]
        public int BookId { get; set; }
        public Book? Book { get; set; }

        [Required]
        [StringLength(450)]
        [Display(Name = "User")]
        public string AppUser { get; set; }

        [Required]
        [StringLength(500)]
        public string Comment { get; set; }

        public int? Rating { get; set; }
    }
}
