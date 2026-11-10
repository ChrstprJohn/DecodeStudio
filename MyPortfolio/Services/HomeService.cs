using Microsoft.EntityFrameworkCore;
using MyPortfolio.Data;
using MyPortfolio.Models.ViewModels;

namespace MyPortfolio.Services;

public class HomeService
{
    private readonly ApplicationDbContext _database;

    public HomeService(ApplicationDbContext database)
    {
        _database = database;
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

        model.Projects = await _database.Projects.AsNoTracking().Where(item => item.IsPublished)
            .OrderBy(item => item.SortOrder).ThenBy(item => item.Id).ToListAsync();
        model.Services = await _database.Services.AsNoTracking().Where(item => item.IsPublished)
            .OrderBy(item => item.SortOrder).ThenBy(item => item.Id).ToListAsync();
        model.TeamMembers = await _database.TeamMembers.AsNoTracking().Where(item => item.IsPublished)
            .OrderBy(item => item.SortOrder).ThenBy(item => item.Id).ToListAsync();
        model.SocialLinks = await _database.SocialLinks.AsNoTracking()
            .OrderBy(item => item.SortOrder).ThenBy(item => item.Id).ToListAsync();
        return model;
    }
}
