using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;
using MyPortfolio.Data;
using MyPortfolio.Models.ViewModels;

namespace MyPortfolio.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(ApplicationDbContext database) : Controller
{
    private static readonly TimeSpan ManilaOffset = TimeSpan.FromHours(8);

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var today = ManilaToday();
        var firstDay = today.AddDays(-13);
        var registrations = await ReadRegistrationDaysAsync(firstDay, today);
        var countsByDate = registrations.ToDictionary(day => day.Date, day => day.Registrations);
        var chartDays = Enumerable.Range(0, 14).Select(offset =>
        {
            var date = firstDay.AddDays(offset);
            return new RegistrationDay { Date = date, Registrations = countsByDate.GetValueOrDefault(date) };
        }).ToArray();

        return View("Index", new AnalyticsViewModel
        {
            Today = today,
            Totals = await database.ReadAnalyticsTotalsAsync(),
            Registrations = chartDays
        });
    }

    [HttpGet]
    public Task<IActionResult> ExportToday()
    {
        var today = ManilaToday();
        return ExportRegistrationsAsync(today, today);
    }

    [HttpGet]
    public async Task<IActionResult> ExportRange(DateOnly? from, DateOnly? to)
    {
        if (!ModelState.IsValid || from is null || to is null ||
            from.Value == DateOnly.MinValue || from > to || to > ManilaToday())
        {
            TempData["ErrorMessage"] = "Enter a valid From and To date through today. From must be on or before To.";
            return RedirectToAction(nameof(Index));
        }

        return await ExportRegistrationsAsync(from.Value, to.Value);
    }

    private async Task<IActionResult> ExportRegistrationsAsync(DateOnly from, DateOnly to)
    {
        var days = await ReadRegistrationDaysAsync(from, to);
        var csv = new StringBuilder("Date (Asia/Manila),Registrations\r\n");
        var countsByDate = days.ToDictionary(day => day.Date, day => day.Registrations);
        for (var date = from; date <= to; date = date.AddDays(1))
        {
            csv.Append(CultureInfo.InvariantCulture, $"{date:yyyy-MM-dd},{countsByDate.GetValueOrDefault(date)}\r\n");
        }

        return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv; charset=utf-8",
            FormattableString.Invariant($"registrations-{from:yyyy-MM-dd}-to-{to:yyyy-MM-dd}.csv"));
    }

    private Task<List<RegistrationDay>> ReadRegistrationDaysAsync(DateOnly from, DateOnly to) =>
        database.ReadRegistrationDaysAsync(ToUtc(from), ToUtc(to.AddDays(1)));

    private static DateOnly ManilaToday() => DateOnly.FromDateTime(DateTimeOffset.UtcNow.ToOffset(ManilaOffset).DateTime);

    private static DateTime ToUtc(DateOnly date) =>
        new DateTimeOffset(date.ToDateTime(TimeOnly.MinValue), ManilaOffset).UtcDateTime;
}
