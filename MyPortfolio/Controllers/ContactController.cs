using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Models.ViewModels;
using MyPortfolio.Services;

namespace MyPortfolio.Controllers;

[Authorize(Roles = "Admin")]
public class ContactController(SiteContentService service) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index() => View(await service.GetContactAsync());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        await service.SaveContactAsync(model);
        TempData["SuccessMessage"] = "Contact details saved.";
        return RedirectToAction(nameof(Index));
    }
}
