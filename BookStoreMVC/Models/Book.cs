using System.ComponentModel.DataAnnotations;

namespace BookStoreMVC.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Display(Name = "Year Published")]
        public int? YearPublished;

        [Display(Name ="Number of pages")]
        public int? NumPages { get; set; }

        public string? Description { get; set; }

        [StringLength(50)]
        public string? Publisher { get; set; }

        [Display(Name = "Front Page")]
        public string? FrontPage { get; set; }

        [Display(Name = "Download URL")]
        public string? DownloadUrl { get; set; }

        [Display(Name = "Author")]
        [Required]
        public int AuthorId { get; set; }
        public Author? Author { get; set; }

        public ICollection<Review>? Reviews { get; set; }
        public ICollection<UserBooks>? Users { get; set; }
        public ICollection<BookGenre>? BookGenres { get; set;}
    }
}
