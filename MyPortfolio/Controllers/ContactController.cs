using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Models.ViewModels;
using MyPortfolio.Services;

namespace MyPortfolio.Controllers;

[Authorize(Roles = "Admin")]
[RequestSizeLimit(3 * 1024 * 1024)]
public class ContactController(SiteContentService service) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index() => View(await service.GetContactAsync());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await service.SaveContactAsync(model);
            }
            catch (ArgumentException exception)
            {
                ModelState.AddModelError(nameof(model.BackgroundImageFile), exception.Message);
            }
        }
        if (!ModelState.IsValid)
        {
            model.BackgroundImagePath = (await service.GetContactAsync()).BackgroundImagePath;
            return View(model);
        }
        TempData["SuccessMessage"] = "Contact details saved.";
        return RedirectToAction(nameof(Index));
    }
}
