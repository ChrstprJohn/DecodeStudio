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
        if (!string.Equals(database, "portfolio_school", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("The school branch requires the portfolio_school database.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
    }
}
