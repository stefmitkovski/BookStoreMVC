using BookStoreMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStoreMVC.ViewModels
{
    public class CreateBookGenreViewModel
    {
        public Book Book { get; set; }
        public IFormFile? Image { get; set; }
        public IFormFile? PDF { get; set; }
        public IEnumerable<int>? SelectedGenres { get; set; }
        public IEnumerable<SelectListItem>? GenreList { get; set; }
    }
}
