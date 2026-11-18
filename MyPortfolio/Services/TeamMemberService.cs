using MyPortfolio.Data;
using MyPortfolio.Models.Entities;
using MyPortfolio.Models.ViewModels;

namespace MyPortfolio.Services;

public class TeamMemberService
{
    private readonly ApplicationDbContext _database;
    private readonly ImageUploadService _images;

    public TeamMemberService(ApplicationDbContext database, ImageUploadService images)
    {
        _database = database;
        _images = images;
    }

    public async Task<List<TeamMember>> GetAllAsync()
    {
        return await _database.ReadTeamMembersAsync();
    }

    public async Task<TeamMember?> GetAsync(int id)
    {
        return await _database.ReadTeamMemberAsync(id);
    }

    public async Task CreateAsync(TeamMemberViewModel model)
    {
        var item = new TeamMember();
        var uploaded = await ApplyFormAndUploadImageAsync(item, model);
        _database.TeamMembers.Add(item);
        await SaveImageChangesAsync(null, item.PhotoPath, uploaded);
    }

    public async Task<bool> UpdateAsync(TeamMemberViewModel model)
    {
        var item = await _database.ReadTeamMemberAsync(model.Id);
        if (item is null)
        {
            return false;
        }

        var previousImage = item.PhotoPath;
        var uploaded = await ApplyFormAndUploadImageAsync(item, model);
        await SaveImageChangesAsync(previousImage, item.PhotoPath, uploaded);
        return true;
    }

    private async Task<string?> ApplyFormAndUploadImageAsync(TeamMember item, TeamMemberViewModel model)
    {
        var uploaded = model.ImageFile is null ? null : await _images.SaveAsync(model.ImageFile);
        item.PhotoPath = uploaded ?? (model.RemoveImage ? null : item.PhotoPath);

        model.ApplyTo(item);
        return uploaded;
    }

    private async Task SaveImageChangesAsync(string? previousImage, string? currentImage, string? uploaded)
    {
        try
        {
            await _database.SaveChangesAsync();
        }
        catch
        {
            if (uploaded is not null) _images.DeleteGenerated(uploaded);
            throw;
        }
        if (previousImage is not null && previousImage != currentImage)
            _images.DeleteGenerated(previousImage);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _database.ReadTeamMemberAsync(id);
        if (item is null)
        {
            return false;
        }
        _database.TeamMembers.Remove(item);
        await _database.SaveChangesAsync();
        if (item.PhotoPath is not null) _images.DeleteGenerated(item.PhotoPath);
        return true;
    }
}
