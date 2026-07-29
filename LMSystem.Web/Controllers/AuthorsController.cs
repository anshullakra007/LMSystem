using LMSystem.Web.Interfaces;
using LMSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LMSystem.Web.Controllers;

[Authorize(Roles = "Admin,Librarian")]
public class AuthorsController : Controller
{
    private readonly IRepository<Author> _repository;

    public AuthorsController(IRepository<Author> repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> Index(string? searchTerm, int page = 1)
    {
        int pageSize = 10;
        var query = _repository.AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(a => a.Name.Contains(searchTerm) || (a.Biography != null && a.Biography.Contains(searchTerm)));
        }

        query = query.OrderBy(a => a.Name);

        var totalItems = await query.CountAsync();
        var authors = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        ViewBag.CurrentSearch = searchTerm;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        return View(authors);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Author entity)
    {
        if (ModelState.IsValid)
        {
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(entity);
    }

    public async Task<IActionResult> Details(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return NotFound();
        return View(entity);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return NotFound();
        return View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Author entity)
    {
        if (id != entity.Id) return NotFound();

        if (ModelState.IsValid)
        {
            _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(entity);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return NotFound();
        return View(entity);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity != null)
        {
            _repository.Remove(entity);
            await _repository.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
