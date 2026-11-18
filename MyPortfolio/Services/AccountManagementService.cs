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

    public Task<List<AccountViewModel>> GetAllAsync(string? status) =>
        _database.ReadAccountsAsync(status switch
        {
            "active" => true,
            "inactive" => false,
            _ => null
        });

    public Task<AccountViewModel?> GetAsync(int id) => _database.ReadAccountAsync(id);

    public async Task<bool> SetActiveAsync(int id, bool isActive, int actingAdminId)
    {
        if (id == actingAdminId && !isActive)
        {
            return false;
        }
        var user = await _database.ReadUserAsync(id);
        if (user is null) return false;
        user.IsActive = isActive;
        await _database.SaveChangesAsync();
        return true;
    }
}
