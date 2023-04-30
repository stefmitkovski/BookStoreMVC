using BookStoreMVC.Data;
using BookStoreMVC.Models;
using BookStoreMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.EntityFrameworkCore;

namespace BookStoreMVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookStoreMVCContext _context;

        public BooksController(BookStoreMVCContext context)
        {
            _context = context;
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
        public async Task<IActionResult> Create(CreateBookGenreViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
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
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "FullName", viewModel.Book.AuthorId);
            return View(viewModel);
        }

            // GET: Books/Edit/5
            public async Task<IActionResult> Edit(int? id)
            {
                if (id == null || _context.Book == null)
                {
                    return NotFound();
                }

                var book = await _context.Book.FindAsync(id);
                if (book == null)
                {
                    return NotFound();
                }
                ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "FullName", book.AuthorId);
                return View(book);
            }

            // POST: Books/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, [Bind("Id,Title,NumPages,Description,Publisher,FrontPage,DownloadUrl,AuthorId")] Book book)
            {
                if (id != book.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(book);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!BookExists(book.Id))
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
                ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "FullName", book.AuthorId);
                return View(book);
            }

            // GET: Books/Delete/5
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

            private bool BookExists(int id)
            {
                return (_context.Book?.Any(e => e.Id == id)).GetValueOrDefault();
            }
        }
    }
