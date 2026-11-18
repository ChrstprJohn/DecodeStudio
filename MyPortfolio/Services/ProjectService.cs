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
        return await _database.ReadProjectsAsync();
    }

    public async Task<Project?> GetAsync(int id)
    {
        return await _database.ReadProjectAsync(id);
    }

    public async Task CreateAsync(ProjectViewModel model)
    {
        var item = new Project();
        var uploaded = await ApplyFormAndUploadImageAsync(item, model);
        _database.Projects.Add(item);
        await SaveImageChangesAsync(null, item.ImagePath, uploaded);
    }

    public async Task<bool> UpdateAsync(ProjectViewModel model)
    {
        var item = await _database.ReadProjectAsync(model.Id);
        if (item is null)
        {
            return false;
        }

        var previousImage = item.ImagePath;
        var uploaded = await ApplyFormAndUploadImageAsync(item, model);
        await SaveImageChangesAsync(previousImage, item.ImagePath, uploaded);
        return true;
    }

    private async Task<string?> ApplyFormAndUploadImageAsync(Project item, ProjectViewModel model)
    {
        if (model.ImageFile is null && string.IsNullOrEmpty(item.ImagePath))
            throw new ArgumentException("Choose a project image.");
        var uploaded = model.ImageFile is null ? null : await _images.SaveAsync(model.ImageFile);
        item.ImagePath = uploaded ?? item.ImagePath;

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
        var item = await _database.ReadProjectAsync(id);
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
