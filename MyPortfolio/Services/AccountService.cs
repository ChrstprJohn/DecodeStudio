using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyPortfolio.Data;
using MyPortfolio.Models.Entities;
using MyPortfolio.Models.ViewModels;

namespace MyPortfolio.Services;

public class AccountService
{
    public static string GetPasswordVersion(User user)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(user.PasswordHash);
        return Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(bytes));
    }

    private readonly ApplicationDbContext _database;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AccountService(ApplicationDbContext database, IPasswordHasher<User> passwordHasher)
    {
        _database = database;
        _passwordHasher = passwordHasher;
    }

    public async Task<User?> LoginAsync(LoginViewModel model)
    {
        var email = model.Email.Trim().ToLowerInvariant();
        var user = await _database.Users.SingleOrDefaultAsync(user => user.Email == email);

        if (user is null || !user.IsActive || user.Role != "Admin")
        {
            return null;
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
            await _database.SaveChangesAsync();
        }

        return user;
    }

    public async Task<User?> GetActiveUserAsync(int id)
    {
        return await _database.Users.AsNoTracking()
            .SingleOrDefaultAsync(user => user.Id == id && user.IsActive);
    }
}
