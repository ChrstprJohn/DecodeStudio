using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Services;

namespace MyPortfolio.Controllers;

public class FaviconController(SiteContentService siteContent) : Controller
{
    [HttpGet("/favicon.ico"), AllowAnonymous, ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Index()
    {
        var path = await siteContent.GetFaviconPathAsync();
        return LocalRedirect(path ?? Url.Content("~/favicon.png"));
    }
}
