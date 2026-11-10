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
            HeroImagePath = item.HeroImagePath,
            HeroLinkText = item.HeroLinkText,
            HeroLinkUrl = item.HeroLinkUrl,
            AboutTitle = item.AboutTitle,
            AboutDescription = item.AboutDescription,
            ContactEmail = item.ContactEmail,
            ContactPhone = item.ContactPhone,
            FooterText = item.FooterText
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
        var previousImage = item.HeroImagePath;
        var uploaded = model.ImageFile is null ? null : await _images.SaveAsync(model.ImageFile);
        item.HeroImagePath = uploaded ?? (model.RemoveImage ? null : item.HeroImagePath);

        item.StudioName = model.StudioName.Trim();
        item.Tagline = model.Tagline?.Trim();
        item.HeroTitle = model.HeroTitle.Trim();
        item.HeroDescription = model.HeroDescription.Trim();
        item.HeroLinkText = model.HeroLinkText?.Trim();
        item.HeroLinkUrl = model.HeroLinkUrl?.Trim();
        item.AboutTitle = model.AboutTitle.Trim();
        item.AboutDescription = model.AboutDescription.Trim();
        item.ContactEmail = model.ContactEmail.Trim();
        item.ContactPhone = model.ContactPhone?.Trim();
        item.FooterText = model.FooterText?.Trim();
        try
        {
            await _database.SaveChangesAsync();
        }
        catch
        {
            if (uploaded is not null) _images.DeleteGenerated(uploaded);
            throw;
        }
        if (previousImage is not null && previousImage != item.HeroImagePath)
            _images.DeleteGenerated(previousImage);
    }

    public async Task DeleteAsync()
    {
        var item = await _database.SiteContent.FindAsync(1);
        if (item is not null)
        {
            _database.SiteContent.Remove(item);
            await _database.SaveChangesAsync();
            if (item.HeroImagePath is not null) _images.DeleteGenerated(item.HeroImagePath);
        }
    }
}
