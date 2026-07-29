using LMSystem.Web.Data;
using LMSystem.Web.Interfaces;
using LMSystem.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace LMSystem.Web.Repositories;

public class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Book>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(b => b.Author)
            .Include(b => b.Category)
            .Include(b => b.Publisher)
            .ToListAsync();
    }

    public async Task<Book?> GetByIdWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(b => b.Author)
            .Include(b => b.Category)
            .Include(b => b.Publisher)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<(IEnumerable<Book> Books, int TotalCount)> GetPagedBooksAsync(int page, int pageSize, string? searchTerm)
    {
        var query = _dbSet
            .Include(b => b.Author)
            .Include(b => b.Category)
            .Include(b => b.Publisher)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(b => b.Title.Contains(searchTerm) || 
                                     b.ISBN.Contains(searchTerm) ||
                                     (b.Author != null && b.Author.Name.Contains(searchTerm)));
        }

        var totalCount = await query.CountAsync();
        
        var books = await query
            .OrderBy(b => b.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (books, totalCount);
    }
}
