using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Models.ViewModels;
using MyPortfolio.Services;

namespace MyPortfolio.Controllers;

[Authorize(Roles = "Admin")]
public class StudioController : Controller
{
    private readonly SiteContentService _service;

    public StudioController(SiteContentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? edit)
    {
        if (edit == "contact")
            return RedirectToAction(nameof(ContactController.Index), "Contact", new { edit });

        ViewData["EditingSection"] = edit;
        return View(await _service.GetAsync());
    }

    [HttpPost, ValidateAntiForgeryToken, RequestSizeLimit(3 * 1024 * 1024)]
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
                ModelState.AddModelError(nameof(model.FaviconFile), exception.Message);
            }
        }
        if (!ModelState.IsValid)
        {
            ViewData["EditingSection"] = "studio";
            model.FaviconPath = await _service.GetFaviconPathAsync();
            return View("Index", model);
        }

        TempData["SuccessMessage"] = "Brand and copy saved.";
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
        TempData["SuccessMessage"] = "Studio and contact details cleared.";
        return RedirectToAction(nameof(Index));
    }
}
