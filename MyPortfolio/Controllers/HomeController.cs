using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Models.ViewModels;
using MyPortfolio.Services;
using System.Diagnostics;

namespace MyPortfolio.Controllers
{
    public class HomeController(HomeService homeService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var model = await homeService.GetHomeAsync();
            ViewData["StudioName"] = model.StudioName;
            return View(model);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
