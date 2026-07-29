using LMSystem.Web.Data;
using LMSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMSystem.Web.Controllers;

[Authorize]
public class BorrowController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public BorrowController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? searchTerm, int page = 1)
    {
        int pageSize = 10;
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var query = _context.BorrowRecords
            .Include(br => br.Book)
            .Include(br => br.User)
            .AsQueryable();

        if (User.IsInRole("Student"))
        {
            query = query.Where(br => br.UserId == user.Id);
        }

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(br => 
                (br.Book != null && br.Book.Title.Contains(searchTerm)) || 
                (br.User != null && br.User.Name.Contains(searchTerm)) ||
                br.Status.Contains(searchTerm));
        }

        query = query.OrderByDescending(br => br.IssueDate);

        var totalItems = await query.CountAsync();
        var records = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        ViewBag.CurrentSearch = searchTerm;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        return View(records);
    }

    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Issue(int bookId)
    {
        var book = await _context.Books.FindAsync(bookId);
        if (book == null || book.AvailableQuantity <= 0)
        {
            TempData["ErrorMessage"] = "Book is not available.";
            return RedirectToAction("Index", "Books");
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        // Check if student already has 5 books
        var currentBorrowCount = await _context.BorrowRecords
            .CountAsync(br => br.UserId == user.Id && br.ReturnDate == null);

        if (currentBorrowCount >= 5)
        {
            TempData["ErrorMessage"] = "You have reached the maximum limit of 5 books.";
            return RedirectToAction("Index", "Books");
        }

        // Check if student has unpaid fines
        var hasUnpaidFines = await _context.BorrowRecords
            .AnyAsync(br => br.UserId == user.Id && br.FineAmount > 0 && br.Status != "FinePaid");

        if (hasUnpaidFines)
        {
            TempData["ErrorMessage"] = "You have unpaid fines. Please pay them before issuing new books.";
            return RedirectToAction("Index", "Books");
        }

        // Check if student already has this book requested or issued
        var existingRecord = await _context.BorrowRecords
            .FirstOrDefaultAsync(br => br.BookId == bookId && br.UserId == user.Id && br.ReturnDate == null);

        if (existingRecord != null)
        {
            TempData["ErrorMessage"] = "You have already requested or issued this book.";
            return RedirectToAction("Index", "Books");
        }

        var record = new BorrowRecord
        {
            BookId = bookId,
            UserId = user.Id,
            IssueDate = DateTime.UtcNow, // Will be updated on actual issue
            DueDate = DateTime.UtcNow.AddDays(14), // Configurable
            Status = "Requested"
        };

        _context.BorrowRecords.Add(record);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Book requested successfully. Please collect from the library.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Librarian")]
    [HttpPost]
    public async Task<IActionResult> ApproveIssue(int id)
    {
        var record = await _context.BorrowRecords.Include(br => br.Book).FirstOrDefaultAsync(br => br.Id == id);
        if (record == null) return NotFound();

        if (record.Status == "Requested" && record.Book != null && record.Book.AvailableQuantity > 0)
        {
            record.Status = "Issued";
            record.IssueDate = DateTime.UtcNow;
            record.DueDate = DateTime.UtcNow.AddDays(14);
            record.Book.AvailableQuantity -= 1;
            
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Book issue approved.";
        }
        else
        {
            TempData["ErrorMessage"] = "Cannot approve issue. Book may be unavailable.";
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Librarian")]
    [HttpPost]
    public async Task<IActionResult> Return(int id)
    {
        var record = await _context.BorrowRecords.Include(br => br.Book).FirstOrDefaultAsync(br => br.Id == id);
        if (record == null) return NotFound();

        if (record.Status == "Issued")
        {
            record.Status = "Returned";
            record.ReturnDate = DateTime.UtcNow;
            
            if (record.Book != null)
            {
                record.Book.AvailableQuantity += 1;
            }

            // Calculate Fine
            var overdueDays = (DateTime.UtcNow - record.DueDate).TotalDays;
            if (overdueDays > 0)
            {
                record.FineAmount = (decimal)Math.Ceiling(overdueDays) * 10; // 10 units fine per day
            }
            
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Book returned successfully. Fine calculated: {record.FineAmount:C}";
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Librarian")]
    [HttpPost]
    public async Task<IActionResult> PayFine(int id)
    {
        var record = await _context.BorrowRecords.FindAsync(id);
        if (record == null) return NotFound();

        if (record.FineAmount > 0 && record.Status != "FinePaid")
        {
            record.Status = "FinePaid";
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Fine has been marked as paid.";
        }

        return RedirectToAction(nameof(Index));
    }
}
