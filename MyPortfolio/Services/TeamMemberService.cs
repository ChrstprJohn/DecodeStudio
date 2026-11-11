using Microsoft.EntityFrameworkCore;
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
        return await _database.TeamMembers.AsNoTracking().OrderBy(item => item.SortOrder).ThenBy(item => item.Id).ToListAsync();
    }

    public async Task<TeamMember?> GetAsync(int id)
    {
        return await _database.TeamMembers.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id);
    }

    public async Task<bool> SaveAsync(TeamMemberViewModel model)
    {
        var item = model.Id == 0 ? new TeamMember() : await _database.TeamMembers.FindAsync(model.Id);
        if (item is null)
        {
            return false;
        }

        var previousImage = item.PhotoPath;
        var uploaded = model.ImageFile is null ? null : await _images.SaveAsync(model.ImageFile);
        item.PhotoPath = uploaded ?? (model.RemoveImage ? null : item.PhotoPath);

        item.FirstName = model.FirstName.Trim();
        item.MiddleName = model.MiddleName?.Trim();
        item.LastName = model.LastName.Trim();
        item.RoleTitle = model.RoleTitle.Trim();
        item.Bio = model.Bio.Trim();
        item.SortOrder = model.SortOrder;
        item.IsPublished = model.IsPublished;

        if (model.Id == 0)
        {
            _database.TeamMembers.Add(item);
        }
        try
        {
            await _database.SaveChangesAsync();
        }
        catch
        {
            if (uploaded is not null) _images.DeleteGenerated(uploaded);
            throw;
        }
        if (previousImage is not null && previousImage != item.PhotoPath)
            _images.DeleteGenerated(previousImage);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _database.TeamMembers.FindAsync(id);
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
