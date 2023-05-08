using BookStoreMVC.Data;
using BookStoreMVC.Models;
using BookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Data;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace BookStoreMVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookStoreMVCContext _context;
        private readonly IHostingEnvironment webHostEnvironment;

        public BooksController(BookStoreMVCContext context, IHostingEnvironment hostEnvironment)
        {
            _context = context;
            webHostEnvironment = hostEnvironment;
        }

        // GET: Books
        public async Task<IActionResult> Index(string searchString, string BookGenre)
        {
            IQueryable<BookGenre> books = _context.bookGenres.AsQueryable();
            IQueryable<string> genreQuery = _context.Genre.Distinct().Select(g => g.GenreName).Distinct();

            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => s.Book.Title.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(BookGenre))
            {
                books = books.Where(s => s.Genre.GenreName == BookGenre);
            }

            books = books.Include(b => b.Book).ThenInclude(b => b.Author);

            var bookGenreVM = new BookGenreViewModel
            {
                Genre = new SelectList(await genreQuery.ToListAsync()),
                Books = await books.Select(s => s.Book).Distinct().ToListAsync(),
                Reviews = await _context.Review.ToListAsync()
            };

            return View(bookGenreVM);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            CreateBookGenreViewModel viewModel = new CreateBookGenreViewModel
            {
                Book = new Book(),
                GenreList = new MultiSelectList(_context.Genre.AsEnumerable().OrderBy(s => s.GenreName), "Id", "GenreName"),
                SelectedGenres = Enumerable.Empty<int>()
            };
            ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "FullName");
            return View(viewModel);
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateBookGenreViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.Image != null)
                {
                    string uniqueFileName = UploadedFile(viewModel);
                    viewModel.Book.FrontPage = uniqueFileName;
                }
                if (viewModel.PDF != null)
                {
                    string uniqueFileName = UploadedFile(viewModel);
                    viewModel.Book.DownloadUrl = uniqueFileName;
                }
                _context.Add(viewModel.Book);
                await _context.SaveChangesAsync();
                if (viewModel.SelectedGenres != null)
                {
                    foreach (int genreId in viewModel.SelectedGenres)
                    {
                        _context.bookGenres.Add(new BookGenre { BookId = viewModel.Book.Id, GenreId = genreId });
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "FullName", viewModel.Book.AuthorId);
            return View(viewModel);
        }

        // GET: Books/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = _context.Book.Where(m => m.Id == id).Include(m => m.BookGenres).First();
            if (book == null)
            {
                return NotFound();
            }

            CreateBookGenreViewModel viewModel = new CreateBookGenreViewModel
            {
                Book = book,
                GenreList = new MultiSelectList(_context.Genre.AsEnumerable().OrderBy(s => s.GenreName), "Id", "GenreName"),
                SelectedGenres = book.BookGenres.Select(s => s.GenreId)
            };
            ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "FullName", book.AuthorId);
            return View(viewModel);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, CreateBookGenreViewModel viewModel)
        {
            if (id != viewModel.Book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (viewModel.Image != null)
                    {
                        string uniqueFileName = UploadedFile(viewModel);
                        viewModel.Book.FrontPage = uniqueFileName;
                    }
                    if (viewModel.PDF != null)
                    {
                        string uniqueFileName = UploadedFile(viewModel);
                        viewModel.Book.DownloadUrl = uniqueFileName;
                    }
                    _context.Update(viewModel.Book);
                    await _context.SaveChangesAsync();
                    IEnumerable<int> newGenresList = viewModel.SelectedGenres;
                    IEnumerable<int> prevGenresList = _context.bookGenres.Where(s => s.BookId == id).Select(s => s.GenreId);
                    IQueryable<BookGenre> toBeRemoved = _context.bookGenres.Where(s => s.BookId == id);
                    if (newGenresList != null)
                    {
                        toBeRemoved = toBeRemoved.Where(s => !newGenresList.Contains(s.GenreId));
                        foreach (int genreId in newGenresList)
                        {
                            if (!prevGenresList.Any(s => s == genreId))
                            {
                                _context.bookGenres.Add(new BookGenre { GenreId = genreId, BookId = id });
                            }
                        }
                    }
                    _context.bookGenres.RemoveRange(toBeRemoved);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(viewModel.Book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "FullName", viewModel.Book.AuthorId);
            return View(viewModel);
        }

        // GET: Books/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Book == null)
            {
                return Problem("Entity set 'BookStoreMVCContext.Book'  is null.");
            }
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        private string UploadedFile(CreateBookGenreViewModel model)
        {
            string uniqueFileName = null;

            if (model.Image != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.Image.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Image.CopyTo(fileStream);
                }
                model.Image = null;
                return uniqueFileName;
            }
            else if (model.PDF != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "pdfs");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.PDF.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.PDF.CopyTo(fileStream);
                }
                return uniqueFileName;
            }
            return null;
        }

        private bool BookExists(int id)
        {
            return (_context.Book?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
