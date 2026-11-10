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
        return View("Edit", new ProjectViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectViewModel model)
    {
        model.Id = 0;
        if (model.ImageFile is null) ModelState.AddModelError(nameof(model.ImageFile), "Choose a project image.");
        if (!ModelState.IsValid)
        {
            return View("Edit", model);
        }
        try
        {
            await _service.SaveAsync(model);
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(nameof(model.ImageFile), exception.Message);
            return View("Edit", model);
        }
        TempData["SuccessMessage"] = "Project created.";
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
        return View(new ProjectViewModel
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
            return View(model);
        }
        try
        {
            if (!await _service.SaveAsync(model)) return NotFound();
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(nameof(model.ImageFile), exception.Message);
            return View(model);
        }
        TempData["SuccessMessage"] = "Project updated.";
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
        TempData["SuccessMessage"] = "Project deleted.";
        return RedirectToAction(nameof(Index));
    }
}
