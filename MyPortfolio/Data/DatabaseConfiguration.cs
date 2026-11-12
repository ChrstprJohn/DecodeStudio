using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace MyPortfolio.Data;

public static class DatabaseConfiguration
{
    public static void AddDatabase(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Portfolio")
            ?? throw new InvalidOperationException(
                "Configure ConnectionStrings:Portfolio using User Secrets or environment variables.");

        var database = new SqlConnectionStringBuilder(connectionString).InitialCatalog;
        if (!string.Equals(database, "portfolio", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("The public branch requires the portfolio database.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
    }
}
