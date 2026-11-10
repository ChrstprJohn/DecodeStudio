using Microsoft.EntityFrameworkCore;

namespace MyPortfolio.Data;

public static class DatabaseConfiguration
{
    public static void AddDatabase(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Portfolio")
            ?? throw new InvalidOperationException(
                "Configure ConnectionStrings:Portfolio using User Secrets or environment variables.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
    }
}
