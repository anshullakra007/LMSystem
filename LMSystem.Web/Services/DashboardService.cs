using LMSystem.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace LMSystem.Web.Services;

public class DashboardService
{
    private readonly ApplicationDbContext _context;

    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetTotalBooksAsync()
    {
        return await _context.Books.SumAsync(b => b.Quantity);
    }

    public async Task<int> GetTotalStudentsAsync()
    {
        return await _context.UserRoles
            .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur, r })
            .Where(x => x.r.Name == "Student")
            .CountAsync();
    }

    public async Task<int> GetTotalLibrariansAsync()
    {
        return await _context.UserRoles
            .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur, r })
            .Where(x => x.r.Name == "Librarian")
            .CountAsync();
    }

    public async Task<int> GetBooksIssuedAsync()
    {
        return await _context.BorrowRecords.CountAsync(br => br.Status == "Issued");
    }

    public async Task<int> GetBooksReturnedAsync()
    {
        return await _context.BorrowRecords.CountAsync(br => br.Status == "Returned" || br.Status == "FinePaid");
    }

    public async Task<int> GetOverdueBooksAsync()
    {
        return await _context.BorrowRecords.CountAsync(br => br.Status == "Issued" && br.DueDate < DateTime.UtcNow);
    }

    public async Task<decimal> GetTotalFinesCollectedAsync()
    {
        var fines = await _context.BorrowRecords
            .Where(b => b.Status == "FinePaid")
            .Select(b => b.FineAmount)
            .ToListAsync();
            
        return fines.Sum();
    }

    public async Task<IEnumerable<dynamic>> GetRecentBorrowingsAsync()
    {
        return await _context.BorrowRecords
            .Include(b => b.Book)
            .Include(b => b.User)
            .OrderByDescending(b => b.IssueDate)
            .Take(5)
            .Select(b => new { 
                User = b.User != null ? b.User.Name : "Unknown", 
                Book = b.Book != null ? b.Book.Title : "Unknown", 
                Status = b.Status, 
                Date = b.IssueDate 
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<dynamic>> GetRecentlyAddedBooksAsync()
    {
        return await _context.Books
            .Include(b => b.Author)
            .OrderByDescending(b => b.Id)
            .Take(5)
            .Select(b => new { 
                Title = b.Title, 
                Author = b.Author != null ? b.Author.Name : "Unknown", 
                Quantity = b.Quantity, 
                IsAvailable = b.AvailableQuantity > 0,
                ImageUrl = b.ImageUrl 
            })
            .ToListAsync();
    }
}
