using Microsoft.EntityFrameworkCore;
using MyPortfolio.Data;
using MyPortfolio.Models.ViewModels;

namespace MyPortfolio.Services;

public class AccountManagementService
{
    private readonly ApplicationDbContext _database;

    public AccountManagementService(ApplicationDbContext database)
    {
        _database = database;
    }

    public async Task<List<AccountViewModel>> GetAllAsync(string? status)
    {
        var query = _database.Users.AsNoTracking();
        if (status == "active") query = query.Where(user => user.IsActive);
        if (status == "inactive") query = query.Where(user => !user.IsActive);

        return await query.OrderByDescending(user => user.CreatedAtUtc)
            .Select(user => new AccountViewModel
            {
                Id = user.Id, FirstName = user.FirstName, MiddleName = user.MiddleName,
                LastName = user.LastName, Email = user.Email, Role = user.Role,
                IsActive = user.IsActive, CreatedAtUtc = user.CreatedAtUtc,
                ProfileImagePath = user.ProfileImagePath
            }).ToListAsync();
    }

    public async Task<AccountViewModel?> GetAsync(int id)
    {
        return await _database.Users.AsNoTracking().Where(user => user.Id == id)
            .Select(user => new AccountViewModel
            {
                Id = user.Id, FirstName = user.FirstName, MiddleName = user.MiddleName,
                LastName = user.LastName, Email = user.Email, Role = user.Role,
                IsActive = user.IsActive, CreatedAtUtc = user.CreatedAtUtc,
                ProfileImagePath = user.ProfileImagePath
            }).SingleOrDefaultAsync();
    }

    public async Task<bool> SetActiveAsync(int id, bool isActive, int actingAdminId)
    {
        if (id == actingAdminId && !isActive)
        {
            return false;
        }
        var user = await _database.Users.FindAsync(id);
        if (user is null) return false;
        user.IsActive = isActive;
        await _database.SaveChangesAsync();
        return true;
    }
}
