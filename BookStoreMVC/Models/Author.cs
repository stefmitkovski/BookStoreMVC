using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreMVC.Models
{
    public class Author
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Birth Date")]
        public DateTime? BirthDate { get; set; }

        [StringLength(50)]
        public string? Nationality { get; set; }

        [StringLength(50)]
        public string? Gender { get; set; }

        [NotMapped]
        public string FullName { get { return String.Format("{0} {1}", FirstName, LastName); } }
        public ICollection<Book>? Books { get; set;}

    }
}
