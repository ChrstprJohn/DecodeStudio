using Microsoft.EntityFrameworkCore;
using MyPortfolio.Data;
using MyPortfolio.Models.Entities;
using MyPortfolio.Models.ViewModels;

namespace MyPortfolio.Services;

public class SiteContentService
{
    private readonly ApplicationDbContext _database;
    private readonly ImageUploadService _images;

    public SiteContentService(ApplicationDbContext database, ImageUploadService images)
    {
        _database = database;
        _images = images;
    }

    public async Task<SiteContentViewModel> GetAsync()
    {
        var item = await _database.SiteContent.AsNoTracking().SingleOrDefaultAsync(item => item.Id == 1);
        if (item is null)
        {
            return new SiteContentViewModel();
        }
        return new SiteContentViewModel
        {
            StudioName = item.StudioName,
            Tagline = item.Tagline,
            HeroTitle = item.HeroTitle,
            HeroDescription = item.HeroDescription,
            AboutTitle = item.AboutTitle,
            AboutDescription = item.AboutDescription
        };
    }

    public async Task SaveAsync(SiteContentViewModel model)
    {
        var item = await _database.SiteContent.FindAsync(1);
        if (item is null)
        {
            item = new SiteContent { Id = 1 };
            _database.SiteContent.Add(item);
        }
        item.StudioName = model.StudioName.Trim();
        item.Tagline = model.Tagline?.Trim();
        item.HeroTitle = model.HeroTitle.Trim();
        item.HeroDescription = model.HeroDescription.Trim();
        item.AboutTitle = model.AboutTitle.Trim();
        item.AboutDescription = model.AboutDescription.Trim();
        await _database.SaveChangesAsync();
    }

    public async Task<ContactViewModel> GetContactAsync()
    {
        return await _database.SiteContent.AsNoTracking().Where(item => item.Id == 1)
            .Select(item => new ContactViewModel
            {
                Email = item.ContactEmail,
                Phone = item.ContactPhone,
                BackgroundImagePath = item.ContactBackgroundImagePath
            })
            .SingleOrDefaultAsync() ?? new ContactViewModel();
    }

    public async Task SaveContactAsync(ContactViewModel model)
    {
        var item = await _database.SiteContent.FindAsync(1);
        if (item is null)
        {
            if (string.IsNullOrWhiteSpace(model.Email) && string.IsNullOrWhiteSpace(model.Phone) &&
                model.BackgroundImageFile is null) return;
            var defaults = new HomeViewModel();
            item = new SiteContent { Id = 1, StudioName = defaults.StudioName, HeroTitle = defaults.HeroTitle };
            _database.SiteContent.Add(item);
        }
        item.ContactEmail = model.Email?.Trim() ?? "";
        item.ContactPhone = string.IsNullOrWhiteSpace(model.Phone) ? null : model.Phone.Trim();
        var previousImage = item.ContactBackgroundImagePath;
        var uploaded = model.BackgroundImageFile is null ? null : await _images.SaveAsync(model.BackgroundImageFile);
        item.ContactBackgroundImagePath = uploaded ?? (model.RemoveBackgroundImage ? null : previousImage);
        try
        {
            await _database.SaveChangesAsync();
        }
        catch
        {
            if (uploaded is not null) _images.DeleteGenerated(uploaded);
            throw;
        }
        if (previousImage is not null && previousImage != item.ContactBackgroundImagePath)
            _images.DeleteGenerated(previousImage);
    }

    public async Task DeleteAsync()
    {
        var item = await _database.SiteContent.FindAsync(1);
        if (item is not null)
        {
            _database.SiteContent.Remove(item);
            await _database.SaveChangesAsync();
            if (item.ContactBackgroundImagePath is not null) _images.DeleteGenerated(item.ContactBackgroundImagePath);
        }
    }
}
