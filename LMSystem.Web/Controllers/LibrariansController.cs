using LMSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LMSystem.Web.Controllers;

[Authorize(Roles = "Admin")]
public class LibrariansController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public LibrariansController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.GetUsersInRoleAsync("Librarian");
        return View(users);
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
                await _userManager.AddToRoleAsync(user, "Librarian");
                TempData["SuccessMessage"] = "Librarian created successfully.";
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
        return View(user);
    }

    public async Task<IActionResult> Delete(string id)
    {
        if (id == null) return NotFound();
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();
        return View(user);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
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
