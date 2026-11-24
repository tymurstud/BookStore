using BookStore.Web.Data;
using BookStore.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Web.Services;

public class BookService : IBookService
{
    private readonly AppDbContext _db;

    public BookService(AppDbContext db) => _db = db;

    public async Task<PagedResult<Book>> GetPageAsync(int page, int pageSize, string? search = null, string? sort = null, string? dir = null)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        IQueryable<Book> q = _db.Books.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            q = q.Where(b => b.Title.Contains(term) || b.Isbn.Contains(term));
        }

        var isDesc = string.Equals(dir, "desc", StringComparison.OrdinalIgnoreCase);

        q = sort?.ToLowerInvariant() switch
        {
            "price" => isDesc ? q.OrderByDescending(b => b.Price) : q.OrderBy(b => b.Price),
            "date" => isDesc ? q.OrderByDescending(b => b.PublicationDate) : q.OrderBy(b => b.PublicationDate),
            _ => isDesc ? q.OrderByDescending(b => b.Title) : q.OrderBy(b => b.Title)
        };

        var total = await q.CountAsync();
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResult<Book>(items, page, pageSize, total);
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        return await _db.Books.FindAsync(id);
    }

    public async Task<Book> AddAsync(Book book)
    {
        _db.Books.Add(book);
        await _db.SaveChangesAsync();
        return book;
    }

    public async Task<bool> UpdateAsync(Book book)
    {
        var existing = await _db.Books.FindAsync(book.Id);
        if (existing == null) return false;

        existing.Title = book.Title;
        existing.Isbn = book.Isbn;
        existing.PublicationDate = book.PublicationDate;
        existing.NumPages = book.NumPages;
        existing.Price = book.Price;
        existing.Author = book.Author;
        existing.Publisher = book.Publisher;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var book = await _db.Books.FindAsync(id);
        if (book == null) return false;

        _db.Books.Remove(book);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsbnExistsAsync(string isbn, int? excludeId = null)
    {
        return await _db.Books
            .AsNoTracking()
            .AnyAsync(b => b.Isbn == isbn && (!excludeId.HasValue || b.Id != excludeId.Value));
    }
}
