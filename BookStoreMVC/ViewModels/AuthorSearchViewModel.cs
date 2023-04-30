using BookStoreMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStoreMVC.ViewModels
{
    public class AuthorSearchViewModel
    {
        public IList<Author> Authors { get; set; }
        public SelectList Nationalities { get; set; }
        public string nationalityString { get; set; }
        public string SearchString { get; set; }
    }
}
