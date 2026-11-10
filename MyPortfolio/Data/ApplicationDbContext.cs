using Microsoft.EntityFrameworkCore;
using MyPortfolio.Models.Entities;

namespace MyPortfolio.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<SiteContent> SiteContent => Set<SiteContent>();
}
