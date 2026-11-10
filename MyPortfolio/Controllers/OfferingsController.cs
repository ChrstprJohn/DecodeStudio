using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Models.ViewModels;
using MyPortfolio.Services;

namespace MyPortfolio.Controllers;

[Authorize(Roles = "Admin")]
[RequestSizeLimit(3 * 1024 * 1024)]
public class OfferingsController : Controller
{
    private readonly ServiceOfferingService _service;

    public OfferingsController(ServiceOfferingService service)
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
        return View("Edit", new ServiceOfferingViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServiceOfferingViewModel model)
    {
        model.Id = 0;
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
        TempData["SuccessMessage"] = "Service created.";
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
        return View(new ServiceOfferingViewModel
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
            ImagePath = item.ImagePath,
            SortOrder = item.SortOrder,
            IsPublished = item.IsPublished
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ServiceOfferingViewModel model)
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
        TempData["SuccessMessage"] = "Service updated.";
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
        TempData["SuccessMessage"] = "Service deleted.";
        return RedirectToAction(nameof(Index));
    }
}

