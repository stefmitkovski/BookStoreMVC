using BookStoreMVC.Models;

namespace BookStoreMVC.ViewModels
{
    public class BookDetails
    {
        public Book Book;
        public IList<Review> Reviews;
        public string Purchesed;
    }
}
