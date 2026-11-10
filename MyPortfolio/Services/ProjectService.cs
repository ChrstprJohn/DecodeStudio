using Microsoft.EntityFrameworkCore;
using MyPortfolio.Data;
using MyPortfolio.Models.Entities;
using MyPortfolio.Models.ViewModels;

namespace MyPortfolio.Services;

public class ProjectService
{
    private readonly ApplicationDbContext _database;
    private readonly ImageUploadService _images;

    public ProjectService(ApplicationDbContext database, ImageUploadService images)
    {
        _database = database;
        _images = images;
    }

    public async Task<List<Project>> GetAllAsync()
    {
        return await _database.Projects.AsNoTracking().OrderBy(item => item.SortOrder).ThenBy(item => item.Id).ToListAsync();
    }

    public async Task<Project?> GetAsync(int id)
    {
        return await _database.Projects.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id);
    }

    public async Task<bool> SaveAsync(ProjectViewModel model)
    {
        var item = model.Id == 0 ? new Project() : await _database.Projects.FindAsync(model.Id);
        if (item is null)
        {
            return false;
        }

        var previousImage = item.ImagePath;
        if (model.ImageFile is null && string.IsNullOrEmpty(previousImage))
            throw new ArgumentException("Choose a project image.");
        var uploaded = model.ImageFile is null ? null : await _images.SaveAsync(model.ImageFile);
        item.ImagePath = uploaded ?? item.ImagePath;

        item.Title = model.Title.Trim();
        item.Description = model.Description.Trim();
        item.WebsiteUrl = model.WebsiteUrl.Trim();
        item.SortOrder = model.SortOrder;
        item.IsPublished = model.IsPublished;

        if (model.Id == 0)
        {
            _database.Projects.Add(item);
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
        if (previousImage is not null && previousImage != item.ImagePath)
            _images.DeleteGenerated(previousImage);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _database.Projects.FindAsync(id);
        if (item is null)
        {
            return false;
        }
        _database.Projects.Remove(item);
        await _database.SaveChangesAsync();
        if (item.ImagePath is not null) _images.DeleteGenerated(item.ImagePath);
        return true;
    }
}
