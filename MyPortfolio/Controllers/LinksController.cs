using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Models;
using MyPortfolio.Models.ViewModels;
using MyPortfolio.Services;

namespace MyPortfolio.Controllers;

[Authorize(Roles = "Admin")]
public class LinksController : Controller
{
    private readonly SocialLinkService _service;

    public LinksController(SocialLinkService service)
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
        return View("Edit", new SocialLinkViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SocialLinkViewModel model)
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
            ModelState.AddModelError(nameof(model.CustomIconFile), exception.Message);
            return View("Edit", model);
        }
        TempData["SuccessMessage"] = "Social link created.";
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
        return View(new SocialLinkViewModel
        {
            Id = item.Id,
            Label = item.Label,
            Url = item.Url,
            CurrentIconSource = SocialIcon.Source(item.CustomIconSvg),
            SortOrder = item.SortOrder
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SocialLinkViewModel model)
    {
        if (model.Id <= 0)
        {
            return NotFound();
        }
        model.CurrentIconSource = SocialIcon.Source((await _service.GetAsync(model.Id))?.CustomIconSvg);
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        try
        {
            if (!await _service.SaveAsync(model))
                return NotFound();
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(nameof(model.CustomIconFile), exception.Message);
            return View(model);
        }
        TempData["SuccessMessage"] = "Social link updated.";
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
        TempData["SuccessMessage"] = "Social link deleted.";
        return RedirectToAction(nameof(Index));
    }
}
