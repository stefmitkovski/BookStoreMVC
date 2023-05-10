using BookStoreMVC.Areas.Identity.Data;
using BookStoreMVC.Data;
using BookStoreMVC.Models;
using BookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<BookStoreMVCUser> _userManager;

        public BooksController(BookStoreMVCContext context, IHostingEnvironment hostEnvironment, UserManager<BookStoreMVCUser> userManager)
        {
            _context = context;
            webHostEnvironment = hostEnvironment;
            _userManager = userManager;
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

        // GET: My Books
        [Authorize(Roles = "User")]
        public async Task<IActionResult> MyBooks(string searchString)
        {
            var usr = await _userManager.GetUserAsync(HttpContext.User);
            IQueryable<UserBooks> books = _context.userBooks.AsQueryable().Where(s => s.AppUser == usr.Email);
            IQueryable<string> genreQuery = _context.Genre.Distinct().Select(g => g.GenreName).Distinct();
            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => s.Book.Title.Contains(searchString));
            }

            books = books.Include(b => b.Book).ThenInclude(b => b.Author);

            var bookGenreVM = new BookGenreViewModel
            {
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

            // Do you own this book ?
            string purchesed = "No";
            var usr = await _userManager.GetUserAsync(HttpContext.User);
            if (usr != null)
            {
                var do_you_own_it = await _context.userBooks.Where(b => b.BookId == id && b.AppUser == usr.Email).FirstOrDefaultAsync();
                if (do_you_own_it != null)
                {
                    purchesed = "Yes";
                }
                ViewBag.Email = usr.Email;
            }
            else
            {
                ViewBag.Email = null;
            }
            // Get the reviews of this book
            var all_review = await _context.Review.Where(b => b.BookId == id).ToListAsync();

            var bookDetailsVM = new BookDetails
            {
                Book = book,
                Reviews = all_review,
                Purchesed = purchesed
            };
            return View(bookDetailsVM);
        }

        // GET: Books/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            CreateBookGenreViewModel viewModel = new CreateBookGenreViewModel
            {
                Book = new Book(),
                GenreList = new MultiSelectList(_context.Genre.AsEnumerable(), "Id", "GenreName"),
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Buy(int id)
        {
            if (id == null)
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


            var usr = await _userManager.GetUserAsync(HttpContext.User);
            if (usr == null)
            {
                return NotFound();
            }

            var ownAlready = await _context.userBooks.Where(s => s.AppUser == usr.Email && s.BookId == id).FirstOrDefaultAsync();
            if (ownAlready != null)
            {
                return RedirectToAction(nameof(Index));
            }

            _context.userBooks.Add(new UserBooks { AppUser = usr.Email, BookId = id });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> ReturnBook(int id)
        {
            var usr = await _userManager.GetUserAsync(HttpContext.User);
            if (id == null)
            {
                return NotFound(ModelState);
            }

            var book = _context.userBooks.Where(s => s.BookId == id && s.AppUser == usr.Email).FirstOrDefault();
            if (book == null)
            {
                return RedirectToAction(nameof(Index));
            }

            _context.userBooks.Remove(book);
            _context.SaveChanges();
            return RedirectToAction(nameof(MyBooks));
        }


        private bool BookExists(int id)
        {
            return (_context.Book?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
