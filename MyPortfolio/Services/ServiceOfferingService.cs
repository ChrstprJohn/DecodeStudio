using Microsoft.EntityFrameworkCore;
using MyPortfolio.Data;
using MyPortfolio.Models.Entities;
using MyPortfolio.Models.ViewModels;

namespace MyPortfolio.Services;

public class ServiceOfferingService
{
    private readonly ApplicationDbContext _database;
    private readonly ImageUploadService _images;

    public ServiceOfferingService(ApplicationDbContext database, ImageUploadService images)
    {
        _database = database;
        _images = images;
    }

    public async Task<List<ServiceOffering>> GetAllAsync()
    {
        return await _database.Services.AsNoTracking().OrderBy(item => item.SortOrder).ThenBy(item => item.Id).ToListAsync();
    }

    public async Task<ServiceOffering?> GetAsync(int id)
    {
        return await _database.Services.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id);
    }

    public async Task<bool> SaveAsync(ServiceOfferingViewModel model)
    {
        var item = model.Id == 0 ? new ServiceOffering() : await _database.Services.FindAsync(model.Id);
        if (item is null)
        {
            return false;
        }

        var previousImage = item.ImagePath;
        var uploaded = model.ImageFile is null ? null : await _images.SaveAsync(model.ImageFile);
        item.ImagePath = uploaded ?? item.ImagePath;

        item.Title = model.Title.Trim();
        item.Description = model.Description.Trim();
        item.SortOrder = model.SortOrder;
        item.IsPublished = model.IsPublished;

        if (model.Id == 0)
        {
            _database.Services.Add(item);
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
        var item = await _database.Services.FindAsync(id);
        if (item is null)
        {
            return false;
        }
        _database.Services.Remove(item);
        await _database.SaveChangesAsync();
        if (item.ImagePath is not null) _images.DeleteGenerated(item.ImagePath);
        return true;
    }
}

