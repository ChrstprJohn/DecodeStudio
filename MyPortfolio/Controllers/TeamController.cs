using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Models.ViewModels;
using MyPortfolio.Services;

namespace MyPortfolio.Controllers;

[Authorize(Roles = "Admin")]
[RequestSizeLimit(3 * 1024 * 1024)]
public class TeamController : Controller
{
    private readonly TeamMemberService _service;

    public TeamController(TeamMemberService service)
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
        return View("Edit", new TeamMemberViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TeamMemberViewModel model)
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
        TempData["SuccessMessage"] = "Team member created.";
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
        return View(new TeamMemberViewModel
        {
            Id = item.Id,
            FirstName = item.FirstName,
            MiddleName = item.MiddleName,
            LastName = item.LastName,
            RoleTitle = item.RoleTitle,
            Bio = item.Bio,
            PhotoPath = item.PhotoPath,
            SortOrder = item.SortOrder,
            IsPublished = item.IsPublished
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TeamMemberViewModel model)
    {
        if (model.Id <= 0)
        {
            return NotFound();
        }
        var current = await _service.GetAsync(model.Id);
        if (current is null) return NotFound();
        model.PhotoPath = current.PhotoPath;
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
        TempData["SuccessMessage"] = "Team member updated.";
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
        TempData["SuccessMessage"] = "Team member deleted.";
        return RedirectToAction(nameof(Index));
    }
}
