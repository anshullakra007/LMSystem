#!/bin/bash
ENTITIES=("Category" "Author" "Publisher" "Magazine" "Newspaper")
PLURALS=("Categories" "Authors" "Publishers" "Magazines" "Newspapers")

# Add Edit, Details, Delete to controllers
for i in "${!ENTITIES[@]}"; do
    ENTITY="${ENTITIES[$i]}"
    PLURAL="${PLURALS[$i]}"
    CONTROLLER="LMSystem.Web/Controllers/${PLURAL}Controller.cs"
    
    # We will just replace the whole controller to have full CRUD
    cat << CS_EOF > "$CONTROLLER"
using LMSystem.Web.Interfaces;
using LMSystem.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LMSystem.Web.Controllers;

[Authorize(Roles = "Admin,Librarian")]
public class ${PLURAL}Controller : Controller
{
    private readonly IRepository<${ENTITY}> _repository;

    public ${PLURAL}Controller(IRepository<${ENTITY}> repository)
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
    public async Task<IActionResult> Create(${ENTITY} entity)
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
    public async Task<IActionResult> Edit(int id, ${ENTITY} entity)
    {
        if (id != entity.Id) return NotFound();

        if (ModelState.IsValid)
        {
            await _repository.UpdateAsync(entity);
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
            await _repository.DeleteAsync(entity);
            await _repository.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
CS_EOF

done
