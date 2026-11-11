using Microsoft.EntityFrameworkCore;
using MyPortfolio.Data;
using MyPortfolio.Models;
using MyPortfolio.Models.Entities;
using MyPortfolio.Models.ViewModels;

namespace MyPortfolio.Services;

public class SocialLinkService
{
    private readonly ApplicationDbContext _database;

    public SocialLinkService(ApplicationDbContext database)
    {
        _database = database;
    }

    public async Task<List<SocialLink>> GetAllAsync()
    {
        return await _database.SocialLinks.AsNoTracking().OrderBy(item => item.SortOrder).ThenBy(item => item.Id).ToListAsync();
    }

    public async Task<SocialLink?> GetAsync(int id)
    {
        return await _database.SocialLinks.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id);
    }

    public async Task<bool> SaveAsync(SocialLinkViewModel model)
    {
        var item = model.Id == 0 ? new SocialLink() : await _database.SocialLinks.FindAsync(model.Id);
        if (item is null)
        {
            return false;
        }

        item.Label = model.Label.Trim();
        item.Url = model.Url.Trim();
        item.SortOrder = model.SortOrder;
        item.CustomIconSvg = model.CustomIconFile is { } file ? SocialIcon.Read(file)
            : model.RemoveCustomIcon ? null : item.CustomIconSvg;

        if (model.Id == 0)
        {
            _database.SocialLinks.Add(item);
        }
        await _database.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _database.SocialLinks.FindAsync(id);
        if (item is null)
        {
            return false;
        }
        _database.SocialLinks.Remove(item);
        await _database.SaveChangesAsync();
        return true;
    }
}
