using LMSystem.Web.Interfaces;
using LMSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LMSystem.Web.Controllers;

[Authorize(Roles = "Admin,Librarian")]
public class MagazinesController : Controller
{
    private readonly IRepository<Magazine> _repository;

    public MagazinesController(IRepository<Magazine> repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _repository.GetAllAsync());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Magazine entity)
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
    public async Task<IActionResult> Edit(int id, Magazine entity)
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
