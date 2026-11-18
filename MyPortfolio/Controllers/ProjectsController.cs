using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Models.ViewModels;
using MyPortfolio.Services;

namespace MyPortfolio.Controllers;

[Authorize(Roles = "Admin")]
[RequestSizeLimit(3 * 1024 * 1024)]
public class ProjectsController : Controller
{
    private readonly ProjectService _service;

    public ProjectsController(ProjectService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(await _service.GetAllAsync());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View("Create", new ProjectViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectViewModel model)
    {
        if (model.ImageFile is null) ModelState.AddModelError(nameof(model.ImageFile), "Choose a project image.");
        if (!ModelState.IsValid)
        {
            return View("Create", model);
        }
        try
        {
            await _service.CreateAsync(model);
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(nameof(model.ImageFile), exception.Message);
            return View("Create", model);
        }
        TempData["SuccessMessage"] = "Product created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetAsync(id);
        if (item is null)
        {
            return NotFound();
        }
        return View("Edit", new ProjectViewModel
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
            ImagePath = item.ImagePath,
            WebsiteUrl = item.WebsiteUrl,
            SortOrder = item.SortOrder,
            IsPublished = item.IsPublished
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProjectViewModel model)
    {
        if (model.Id <= 0)
        {
            return NotFound();
        }
        var current = await _service.GetAsync(model.Id);
        if (current is null) return NotFound();
        model.ImagePath = current.ImagePath;
        if (!ModelState.IsValid)
        {
            return View("Edit", model);
        }
        try
        {
            if (!await _service.UpdateAsync(model)) return NotFound();
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(nameof(model.ImageFile), exception.Message);
            return View("Edit", model);
        }
        TempData["SuccessMessage"] = "Product updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _service.GetAsync(id);
        if (item is null)
        {
            return NotFound();
        }
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, bool confirmed)
    {
        if (!confirmed)
        {
            return BadRequest();
        }
        if (!await _service.DeleteAsync(id))
        {
            return NotFound();
        }
        TempData["SuccessMessage"] = "Product deleted.";
        return RedirectToAction(nameof(Index));
    }
}
