using Microsoft.EntityFrameworkCore;
using MyPortfolio.Data;
using MyPortfolio.Models.ViewModels;

namespace MyPortfolio.Services;

public class HomeService(ApplicationDbContext database)
{
    public async Task<HomeViewModel> GetHomeAsync()
    {
        var content = await database.SiteContent
            .AsNoTracking()
            .SingleOrDefaultAsync(content => content.Id == 1);

        if (content is null)
        {
            return new HomeViewModel();
        }

        return new HomeViewModel
        {
            StudioName = content.StudioName,
            HeroTitle = content.HeroTitle,
            HeroDescription = content.HeroDescription
        };
    }
}
