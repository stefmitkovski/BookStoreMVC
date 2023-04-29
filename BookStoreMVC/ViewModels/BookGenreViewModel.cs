﻿using BookStoreMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStoreMVC.ViewModels
{
    public class BookGenreViewModel
    {
        public IList<Book> Books { get; set; }
        public SelectList Genre { get; set; }
        public string BookGenre { get; set; }
        public string SearchString { get; set; }
    }
}
