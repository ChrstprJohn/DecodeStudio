using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyPortfolio.Data;
using MyPortfolio.Models.Entities;
using MyPortfolio.Models.ViewModels;

namespace MyPortfolio.Services;

public class ProfileService
{
    private readonly ApplicationDbContext _database;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ImageUploadService _images;

    public ProfileService(ApplicationDbContext database, IPasswordHasher<User> passwordHasher, ImageUploadService images)
    {
        _database = database;
        _passwordHasher = passwordHasher;
        _images = images;
    }

    public async Task<ProfileViewModel?> GetAsync(int userId)
    {
        return await _database.Users.AsNoTracking().Where(user => user.Id == userId && user.IsActive)
            .Select(user => new ProfileViewModel
            {
                FirstName = user.FirstName, MiddleName = user.MiddleName,
                LastName = user.LastName, Email = user.Email, ProfileImagePath = user.ProfileImagePath
            }).SingleOrDefaultAsync();
    }

    public async Task<string?> UpdateAsync(int userId, ProfileViewModel model)
    {
        var user = await _database.Users.SingleOrDefaultAsync(user => user.Id == userId && user.IsActive);
        if (user is null) return "Account not found.";
        var email = model.Email.Trim().ToLowerInvariant();
        if (await _database.Users.AnyAsync(other => other.Id != userId && other.Email == email))
            return "This email is already registered.";

        string? uploaded = null;
        if (model.Photo is not null)
        {
            try { uploaded = await _images.SaveAsync(model.Photo); }
            catch (ArgumentException exception) { return exception.Message; }
        }
        var previousImage = user.ProfileImagePath;
        user.FirstName = model.FirstName.Trim();
        user.MiddleName = string.IsNullOrWhiteSpace(model.MiddleName) ? null : model.MiddleName.Trim();
        user.LastName = model.LastName.Trim();
        user.Email = email;
        user.ProfileImagePath = uploaded ?? (model.RemovePhoto ? null : user.ProfileImagePath);

        try
        {
            await _database.SaveChangesAsync();
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            if (uploaded is not null) _images.DeleteGenerated(uploaded);
            return "This email is already registered.";
        }
        catch
        {
            if (uploaded is not null) _images.DeleteGenerated(uploaded);
            throw;
        }
        if (previousImage is not null && previousImage != user.ProfileImagePath)
            _images.DeleteGenerated(previousImage);
        return null;
    }

    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordViewModel model)
    {
        var user = await _database.Users.SingleOrDefaultAsync(user => user.Id == userId && user.IsActive);
        if (user is null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.CurrentPassword) == PasswordVerificationResult.Failed)
            return false;
        user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
        await _database.SaveChangesAsync();
        return true;
    }
}
