using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Services;

namespace MyPortfolio.Controllers;

[Authorize(Roles = "Admin")]
public class AccountsController : Controller
{
    private readonly AccountManagementService _service;

    public AccountsController(AccountManagementService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? status)
    {
        ViewData["AccountStatusFilter"] = status is "active" or "inactive" ? status : null;
        return View(await _service.GetAllAsync(status));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var model = await _service.GetAsync(id);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SetActive(int id, bool isActive)
    {
        var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (id == adminId && !isActive)
        {
            TempData["ErrorMessage"] = "You cannot deactivate your own account.";
            return RedirectToAction(nameof(Details), new { id });
        }
        if (!await _service.SetActiveAsync(id, isActive, adminId)) return NotFound();
        TempData["SuccessMessage"] = isActive ? "Account activated." : "Account deactivated.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
