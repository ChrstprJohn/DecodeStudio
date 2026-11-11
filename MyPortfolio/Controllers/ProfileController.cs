using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Models.ViewModels;
using MyPortfolio.Services;

namespace MyPortfolio.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly ProfileService _service;

    public ProfileController(ProfileService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = await _service.GetAsync(GetUserId());
        return model is null ? NotFound() : View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var model = await _service.GetAsync(GetUserId());
        return model is null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequestSizeLimit(3 * 1024 * 1024)]
    public async Task<IActionResult> Edit(ProfileViewModel model)
    {
        if (ModelState.IsValid)
        {
            var error = await _service.UpdateAsync(GetUserId(), model);
            if (error is null)
            {
                TempData["SuccessMessage"] = "Profile updated.";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", error);
        }
        model.ProfileImagePath = (await _service.GetAsync(GetUserId()))?.ProfileImagePath;
        return View(model);
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        if (!await _service.ChangePasswordAsync(GetUserId(), model))
        {
            ModelState.AddModelError(nameof(model.CurrentPassword), "The current password is incorrect.");
            return View(model);
        }
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["SuccessMessage"] = "Password changed. Log in with your new password.";
        return RedirectToAction("Login", "Account");
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
