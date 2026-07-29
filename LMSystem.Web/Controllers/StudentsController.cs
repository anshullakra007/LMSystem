using LMSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LMSystem.Web.Controllers;

[Authorize(Roles = "Admin,Librarian")]
public class StudentsController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly LMSystem.Web.Data.ApplicationDbContext _context;

    public StudentsController(UserManager<ApplicationUser> userManager, LMSystem.Web.Data.ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> Index(string? searchTerm, int page = 1)
    {
        int pageSize = 10;
        var users = await _userManager.GetUsersInRoleAsync("Student");
        var query = users.AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(u => u.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
                                     (u.Email != null && u.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
        }

        query = query.OrderBy(u => u.Name);

        var totalItems = query.Count();
        var students = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        ViewBag.CurrentSearch = searchTerm;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        return View(students);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Email,Name")] ApplicationUser user, string password)
    {
        if (ModelState.IsValid)
        {
            user.UserName = user.Email;
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Student");
                TempData["SuccessMessage"] = "Student created successfully.";
                return RedirectToAction(nameof(Index));
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(user);
    }

    public async Task<IActionResult> Details(string id)
    {
        if (id == null) return NotFound();
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var borrowHistory = _context.BorrowRecords
            .Where(b => b.UserId == id)
            .Select(b => new {
                BookTitle = b.Book.Title,
                b.IssueDate,
                b.ReturnDate,
                b.DueDate,
                b.Status,
                b.FineAmount
            }).ToList();

        ViewBag.BorrowHistory = borrowHistory;
        return View(user);
    }

    public async Task<IActionResult> Edit(string id)
    {
        if (id == null) return NotFound();
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Email")] ApplicationUser user)
    {
        if (id != user.Id) return NotFound();

        var existingUser = await _userManager.FindByIdAsync(id);
        if (existingUser == null) return NotFound();

        existingUser.Name = user.Name;
        existingUser.Email = user.Email;
        existingUser.UserName = user.Email;

        var result = await _userManager.UpdateAsync(existingUser);
        if (result.Succeeded)
        {
            return RedirectToAction(nameof(Index));
        }
        
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return View(user);
    }

    [Authorize(Roles = "Admin")] // Only Admins can delete
    public async Task<IActionResult> Delete(string id)
    {
        if (id == null) return NotFound();
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();
        return View(user);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }
        return RedirectToAction(nameof(Index));
    }
}
