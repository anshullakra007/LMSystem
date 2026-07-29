using LMSystem.Web.Interfaces;
using LMSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LMSystem.Web.Controllers;

[Authorize]
public class BooksController : Controller
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    public async Task<IActionResult> Index(string? searchTerm, int page = 1)
    {
        int pageSize = 10;
        var model = await _bookService.GetPagedBooksAsync(page, pageSize, searchTerm);
        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null) return NotFound();
        return View(book);
    }

    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Create(Book book)
    {
        if (ModelState.IsValid)
        {
            await _bookService.CreateBookAsync(book);
            TempData["SuccessMessage"] = "Book created successfully.";
            return RedirectToAction(nameof(Index));
        }
        await PopulateDropdownsAsync(book);
        return View(book);
    }

    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null) return NotFound();
        
        await PopulateDropdownsAsync(book);
        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Edit(int id, Book book)
    {
        if (id != book.Id) return NotFound();

        if (ModelState.IsValid)
        {
            await _bookService.UpdateBookAsync(book);
            TempData["SuccessMessage"] = "Book updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        await PopulateDropdownsAsync(book);
        return View(book);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _bookService.DeleteBookAsync(id);
        TempData["SuccessMessage"] = "Book deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync(Book? book = null)
    {
        var authors = await _bookService.GetAllAuthorsAsync();
        var categories = await _bookService.GetAllCategoriesAsync();
        var publishers = await _bookService.GetAllPublishersAsync();

        ViewBag.AuthorId = new SelectList(authors, "Id", "Name", book?.AuthorId);
        ViewBag.CategoryId = new SelectList(categories, "Id", "Name", book?.CategoryId);
        ViewBag.PublisherId = new SelectList(publishers, "Id", "Name", book?.PublisherId);
    }
}
