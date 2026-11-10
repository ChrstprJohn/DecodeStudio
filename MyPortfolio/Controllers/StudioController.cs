using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Models.ViewModels;
using MyPortfolio.Services;

namespace MyPortfolio.Controllers;

[Authorize(Roles = "Admin")]
[RequestSizeLimit(3 * 1024 * 1024)]
public class StudioController : Controller
{
    private readonly SiteContentService _service;

    public StudioController(SiteContentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(await _service.GetAsync());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SiteContentViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _service.SaveAsync(model);
            }
            catch (ArgumentException exception)
            {
                ModelState.AddModelError(nameof(model.ImageFile), exception.Message);
            }
        }
        if (!ModelState.IsValid)
        {
            model.HeroImagePath = (await _service.GetAsync()).HeroImagePath;
            return View("Index", model);
        }
        TempData["SuccessMessage"] = "Studio content saved.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete()
    {
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(bool confirmed)
    {
        if (!confirmed)
        {
            return BadRequest();
        }
        await _service.DeleteAsync();
        TempData["SuccessMessage"] = "Studio content cleared.";
        return RedirectToAction(nameof(Index));
    }
}
