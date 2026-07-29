using LMSystem.Web.Interfaces;
using LMSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LMSystem.Web.Controllers;

[Authorize(Roles = "Admin,Librarian")]
public class CategoriesController : Controller
{
    private readonly IRepository<Category> _repository;

    public CategoriesController(IRepository<Category> repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> Index(string? searchTerm, int page = 1)
    {
        int pageSize = 10;
        var query = _repository.AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(c => c.Name.Contains(searchTerm) || (c.Description != null && c.Description.Contains(searchTerm)));
        }

        query = query.OrderBy(c => c.Name);

        var totalItems = await query.CountAsync();
        var categories = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        ViewBag.CurrentSearch = searchTerm;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        return View(categories);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category entity)
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
    public async Task<IActionResult> Edit(int id, Category entity)
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
