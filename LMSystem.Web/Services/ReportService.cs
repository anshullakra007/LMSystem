using LMSystem.Web.Data;
using LMSystem.Web.Interfaces;
using LMSystem.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace LMSystem.Web.Services;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;

    public ReportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReportsViewModel> GetFullReportAsync()
    {
        var model = new ReportsViewModel();

        // 1. Overall Stats
        model.TotalBooks = await _context.Books.SumAsync(b => b.Quantity);
        model.TotalBorrowed = await _context.BorrowRecords.CountAsync(br => br.Status == "Issued");
        model.OverdueBooks = await _context.BorrowRecords.CountAsync(br => br.Status == "Issued" && br.DueDate < DateTime.UtcNow);
        model.FineCollection = await _context.BorrowRecords
            .Where(b => b.Status == "FinePaid")
            .SumAsync(b => b.FineAmount);

        // 2. Chart Data: Books by Category
        var categoryData = await _context.Books
            .Include(b => b.Category)
            .GroupBy(b => b.Category!.Name)
            .Select(g => new { Category = g.Key, Count = g.Sum(b => b.Quantity) })
            .ToListAsync();
            
        model.BooksByCategory = categoryData.ToDictionary(x => x.Category, x => x.Count);

        // Chart Data: Borrowings by Month (Last 6 Months)
        var sixMonthsAgo = DateTime.UtcNow.AddMonths(-5);
        var borrowingsData = await _context.BorrowRecords
            .Where(b => b.IssueDate >= new DateTime(sixMonthsAgo.Year, sixMonthsAgo.Month, 1))
            .ToListAsync();
            
        var monthlyGroups = borrowingsData
            .GroupBy(b => new { b.IssueDate.Year, b.IssueDate.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new { 
                Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"), 
                Count = g.Count() 
            });

        foreach (var item in monthlyGroups)
        {
            model.BorrowingsByMonth[item.Month] = item.Count;
        }

        // 3. Data Tables
        model.OverdueRecords = await _context.BorrowRecords
            .Include(b => b.User)
            .Include(b => b.Book)
            .Where(br => br.Status == "Issued" && br.DueDate < DateTime.UtcNow)
            .OrderBy(br => br.DueDate)
            .Take(10)
            .Select(b => new { 
                User = b.User != null ? b.User.Name : "Unknown", 
                Book = b.Book != null ? b.Book.Title : "Unknown", 
                DueDate = b.DueDate, 
                DaysOverdue = (DateTime.UtcNow - b.DueDate).Days 
            })
            .ToListAsync();

        var mostBorrowed = await _context.BorrowRecords
            .Include(b => b.Book)
            .GroupBy(b => b.BookId)
            .Select(g => new { 
                BookId = g.Key, 
                Count = g.Count() 
            })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToListAsync();
            
        var bookIds = mostBorrowed.Select(x => x.BookId).ToList();
        var books = await _context.Books.Where(b => bookIds.Contains(b.Id)).ToListAsync();
        
        model.MostBorrowedBooks = mostBorrowed.Select(mb => new {
            Book = books.FirstOrDefault(b => b.Id == mb.BookId)?.Title ?? "Unknown",
            TimesBorrowed = mb.Count
        });

        model.RecentFines = await _context.BorrowRecords
            .Include(b => b.User)
            .Where(br => br.Status == "FinePaid" && br.FineAmount > 0)
            .OrderByDescending(br => br.ReturnDate)
            .Take(10)
            .Select(b => new { 
                User = b.User != null ? b.User.Name : "Unknown", 
                Amount = b.FineAmount, 
                Date = b.ReturnDate 
            })
            .ToListAsync();

        return model;
    }
}
