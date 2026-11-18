using MyPortfolio.Data;
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
        return await _database.ReadSocialLinksAsync();
    }

    public async Task<SocialLink?> GetAsync(int id)
    {
        return await _database.ReadSocialLinkAsync(id);
    }

    public async Task CreateAsync(SocialLinkViewModel model)
    {
        var item = new SocialLink();
        model.ApplyTo(item);
        _database.SocialLinks.Add(item);
        await _database.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(SocialLinkViewModel model)
    {
        var item = await _database.ReadSocialLinkAsync(model.Id);
        if (item is null)
        {
            return false;
        }

        model.ApplyTo(item);
        await _database.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _database.ReadSocialLinkAsync(id);
        if (item is null)
        {
            return false;
        }
        _database.SocialLinks.Remove(item);
        await _database.SaveChangesAsync();
        return true;
    }
}
