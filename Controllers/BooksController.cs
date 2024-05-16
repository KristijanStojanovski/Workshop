using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.View;
using Workshop.Data;
using Workshop.Models;
using static System.Reflection.Metadata.BlobBuilder;

namespace Workshop.Controllers
{
    public class BooksController : Controller
    {
        private readonly WorkshopContext _context;

        public BooksController(WorkshopContext context)
        {
            _context = context;
        }

        // GET: Books

        public async Task<IActionResult> Index(string searchString, int? authorId, List<int> genreIds)
        {
            IQueryable<Book> books = _context.Book.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.Title.Contains(searchString));
            }

            if (authorId.HasValue)
            {
                books = books.Where(b => b.AuthorId == authorId);
            }
            if (genreIds != null && genreIds.Any())
            {
                books = books.Where(b => b.Genres.Any(bg => genreIds.Contains(bg.GenreId)));
            }

            var workshopContext = books
                .Include(b => b.author)
                .Include(b => b.Genres)
                .ThenInclude(b => b.genre);

            var averageRatings = await _context.Review
                .GroupBy(r => r.bookId)
                .Select(g => new
                {
                    BookId = g.Key,
                    AverageRating = g.Average(r => r.rating)
                })
                .ToDictionaryAsync(g => g.BookId, g => g.AverageRating);

    
            ViewBag.AverageRatings = averageRatings;
            ViewBag.Authors = await _context.Author.ToListAsync();
            ViewBag.Genres = await _context.Genre.ToListAsync();

            return View(await workshopContext.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.author)
                .Include(b => b.Genres)
                .ThenInclude(bg => bg.genre)
                .Include(r=> r.reviews)
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
            var authors = _context.Author
                .Select(a => new {
                    Id = a.Id,
                    FullName = a.FirstName + " " + a.LastName
                })
                .ToList();
            ViewData["AuthorId"] = new SelectList(authors, "Id", "FullName");
            var reviews = _context.Review.ToList();
            var genres = _context.Genre.ToList();
            ViewBag.Reviews = reviews;
            ViewBag.Genres = genres;
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,YearPublished,NumPages,Publisher,FrontPage,DownloadUrl,AuthorId")] Book book, int[] GenreIds)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();

                if (GenreIds != null)
                {
                    foreach (var genreId in GenreIds)
                    {
                        _context.Add(new BookGenre { BookId = book.Id, GenreId = genreId });
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "FullName", book.AuthorId);
            ViewBag.Genres = await _context.Genre.ToListAsync();
            return View(book);
        }

        // GET: Books/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Genres)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            var authors = _context.Author
                .Select(a => new {
                    Id = a.Id,
                    FullName = a.FirstName + " " + a.LastName
                })
                .ToList();
            ViewData["AuthorId"] = new SelectList(authors, "Id", "FullName", book.AuthorId);

            ViewBag.Genres = await _context.Genre.ToListAsync();

            return View(book);
        }

        // POST: Books/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,YearPublished,NumPages,Publisher,FrontPage,DownloadUrl,AuthorId")] Book book, int[] GenreIds)
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

                    var existingGenres = await _context.BookGenre.Where(bg => bg.BookId == book.Id).ToListAsync();
                    _context.RemoveRange(existingGenres);

                    if (GenreIds != null)
                    {
                        foreach (var genreId in GenreIds)
                        {
                            _context.Add(new BookGenre { BookId = book.Id, GenreId = genreId });
                        }
                    }

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

            var authors = _context.Author
                .Select(a => new {
                    Id = a.Id,
                    FullName = a.FirstName + " " + a.LastName
                })
                .ToList();
            ViewData["AuthorId"] = new SelectList(authors, "Id", "FullName", book.AuthorId);

            ViewBag.Genres = await _context.Genre.ToListAsync();

            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.author)
                .Include(b => b.Genres)
                .ThenInclude(bg => bg.genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
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
            return _context.Book.Any(e => e.Id == id);
        }
        private async Task<double?> CalculateAverageRating(int bookId)
        {
            var ratings = await _context.Review
                .Where(r => r.bookId == bookId)
                .Select(r => r.rating)
                .ToListAsync();

            if (ratings != null && ratings.Any())
            {
                int totalRating = (int)ratings.Sum();
                double averageRating = (double)totalRating / ratings.Count;
                return averageRating;
            }
            else
            {
                return null;
            }
        }

    }

}
