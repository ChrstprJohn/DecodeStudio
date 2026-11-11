using Microsoft.EntityFrameworkCore;
using MyPortfolio.Data;
using MyPortfolio.Models.ViewModels;

namespace MyPortfolio.Services;

public class HomeService
{
    private readonly ApplicationDbContext _database;
    private readonly IWebHostEnvironment _environment;

    public HomeService(ApplicationDbContext database, IWebHostEnvironment environment)
    {
        _database = database;
        _environment = environment;
    }

    public async Task<HomeViewModel> GetHomeAsync()
    {
        var content = await _database.SiteContent
            .AsNoTracking()
            .SingleOrDefaultAsync(content => content.Id == 1);

        var model = new HomeViewModel();
        if (content is not null)
        {
            model.StudioName = content.StudioName;
            model.HeroTitle = content.HeroTitle;
            model.HeroDescription = content.HeroDescription;
            model.Content = content;
        }

        var projects = await _database.Projects.AsNoTracking()
            .OrderBy(item => item.SortOrder).ThenBy(item => item.Id).ToListAsync();
        model.Projects = projects.Where(project => project.IsPublished).ToList();
        model.HeroPreviews = model.Projects
            .Where(project => !string.IsNullOrWhiteSpace(project.ImagePath))
            .DistinctBy(project => project.ImagePath, StringComparer.OrdinalIgnoreCase)
            .Select(project => (project.ImagePath, project.Title))
            .ToList();

        // A registered image follows its project's publication status.
        var registeredPaths = projects.Select(project => project.ImagePath).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var file in _environment.WebRootFileProvider.GetDirectoryContents("images/projects")
                     .OrderBy(file => file.Name, StringComparer.OrdinalIgnoreCase))
        {
            if (file.IsDirectory || Path.GetExtension(file.Name).ToLowerInvariant() is not
                (".avif" or ".gif" or ".jpeg" or ".jpg" or ".png" or ".svg" or ".webp")) continue;

            var path = $"/images/projects/{Uri.EscapeDataString(file.Name)}";
            if (registeredPaths.Contains(path) || registeredPaths.Contains($"/images/projects/{file.Name}")) continue;

            var title = Path.GetFileNameWithoutExtension(file.Name).Replace('-', ' ').Replace('_', ' ');
            model.HeroPreviews.Add((path, title));
        }
        model.Services = await _database.Services.AsNoTracking().Where(item => item.IsPublished)
            .OrderBy(item => item.SortOrder).ThenBy(item => item.Id).ToListAsync();
        model.TeamMembers = await _database.TeamMembers.AsNoTracking().Where(item => item.IsPublished)
            .OrderBy(item => item.SortOrder).ThenBy(item => item.Id).ToListAsync();
        model.SocialLinks = await _database.SocialLinks.AsNoTracking()
            .OrderBy(item => item.SortOrder).ThenBy(item => item.Id).ToListAsync();
        return model;
    }
}
