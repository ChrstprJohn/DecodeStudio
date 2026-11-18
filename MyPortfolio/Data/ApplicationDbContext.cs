using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyPortfolio.Models.Entities;
using MyPortfolio.Models.ViewModels;

namespace MyPortfolio.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<SiteContent> SiteContent => Set<SiteContent>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ServiceOffering> Services => Set<ServiceOffering>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<SocialLink> SocialLinks => Set<SocialLink>();

    public Task<User?> ReadUserAsync(int id, bool activeOnly = false) =>
        ReadSingleAsync(Users, $"EXEC dbo.Users_Read @Id = {id}, @IsActive = {Flag("active", activeOnly ? true : null)}");

    public Task<User?> ReadUserByEmailAsync(string email) =>
        ReadSingleAsync(Users, $"EXEC dbo.Users_Read @Email = {email}");

    public async Task<bool> EmailExistsAsync(string email, int? excludedId = null)
    {
        var excluded = new SqlParameter("excluded", System.Data.SqlDbType.Int)
        {
            Value = (object?)excludedId ?? DBNull.Value
        };
        var matches = await Database.SqlQuery<int>(
            $"EXEC dbo.Users_FindEmail @Email = {email}, @ExcludedId = {excluded}").ToListAsync();
        return matches.Count != 0;
    }

    public Task<List<AccountViewModel>> ReadAccountsAsync(bool? active) =>
        Database.SqlQuery<AccountViewModel>(
            $"EXEC dbo.Users_ListAccounts @IsActive = {Flag("active", active)}").ToListAsync();

    public async Task<AccountViewModel?> ReadAccountAsync(int id) =>
        (await Database.SqlQuery<AccountViewModel>(
            $"EXEC dbo.Users_ListAccounts @Id = {id}").ToListAsync()).SingleOrDefault();

    public Task<SiteContent?> ReadSiteContentAsync() =>
        ReadSingleAsync(SiteContent, $"EXEC dbo.SiteContent_Read");

    public Task<List<Project>> ReadProjectsAsync() =>
        ReadListAsync(Projects, $"EXEC dbo.Projects_Read");

    public Task<Project?> ReadProjectAsync(int id) =>
        ReadSingleAsync(Projects, $"EXEC dbo.Projects_Read @Id = {id}");

    public Task<List<ServiceOffering>> ReadServicesAsync(bool publishedOnly = false) =>
        ReadListAsync(Services, $"EXEC dbo.Services_Read @PublishedOnly = {publishedOnly}");

    public Task<ServiceOffering?> ReadServiceAsync(int id) =>
        ReadSingleAsync(Services, $"EXEC dbo.Services_Read @Id = {id}");

    public Task<List<TeamMember>> ReadTeamMembersAsync(bool publishedOnly = false) =>
        ReadListAsync(TeamMembers, $"EXEC dbo.TeamMembers_Read @PublishedOnly = {publishedOnly}");

    public Task<TeamMember?> ReadTeamMemberAsync(int id) =>
        ReadSingleAsync(TeamMembers, $"EXEC dbo.TeamMembers_Read @Id = {id}");

    public Task<List<SocialLink>> ReadSocialLinksAsync() =>
        ReadListAsync(SocialLinks, $"EXEC dbo.SocialLinks_Read");

    public Task<SocialLink?> ReadSocialLinkAsync(int id) =>
        ReadSingleAsync(SocialLinks, $"EXEC dbo.SocialLinks_Read @Id = {id}");

    private static SqlParameter Flag(string name, bool? value) =>
        new(name, System.Data.SqlDbType.Bit) { Value = (object?)value ?? DBNull.Value };

    private static Task<List<TEntity>> ReadListAsync<TEntity>(DbSet<TEntity> set, FormattableString command)
        where TEntity : class =>
        set.FromSql(command).AsNoTracking().ToListAsync();

    private static async Task<TEntity?> ReadSingleAsync<TEntity>(DbSet<TEntity> set, FormattableString command)
        where TEntity : class =>
        (await set.FromSql(command).ToListAsync()).SingleOrDefault();

    public async Task<AnalyticsTotals> ReadAnalyticsTotalsAsync() =>
        (await Database.SqlQuery<AnalyticsTotals>($"EXEC dbo.Analytics_ReadTotals").ToListAsync()).Single();

    public Task<List<RegistrationDay>> ReadRegistrationDaysAsync(DateTime startUtc, DateTime endUtc) =>
        Database.SqlQuery<RegistrationDay>(
            $"EXEC dbo.Analytics_ReadRegistrations @StartUtc = {startUtc}, @EndUtc = {endUtc}").ToListAsync();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().HasIndex(user => user.Email)
            .IsUnique().HasDatabaseName("UQ_Users_Email");

        var users = modelBuilder.Entity<User>();
        users.InsertUsingStoredProcedure("Users_Insert", "dbo", procedure =>
        {
            procedure.HasParameter(item => item.CreatedAtUtc);
            procedure.HasParameter(item => item.Email);
            procedure.HasParameter(item => item.FirstName);
            procedure.HasParameter(item => item.IsActive);
            procedure.HasParameter(item => item.LastName);
            procedure.HasParameter(item => item.MiddleName);
            procedure.HasParameter(item => item.PasswordHash);
            procedure.HasParameter(item => item.ProfileImagePath);
            procedure.HasParameter(item => item.Role);
            procedure.HasResultColumn(item => item.Id);
        });
        users.UpdateUsingStoredProcedure("Users_Update", "dbo", procedure =>
        {
            procedure.HasOriginalValueParameter(item => item.Id, parameter => parameter.HasName("Id"));
            procedure.HasParameter(item => item.CreatedAtUtc);
            procedure.HasOriginalValueParameter(item => item.CreatedAtUtc, parameter => parameter.HasName("OriginalCreatedAtUtc"));
            procedure.HasParameter(item => item.Email);
            procedure.HasOriginalValueParameter(item => item.Email, parameter => parameter.HasName("OriginalEmail"));
            procedure.HasParameter(item => item.FirstName);
            procedure.HasOriginalValueParameter(item => item.FirstName, parameter => parameter.HasName("OriginalFirstName"));
            procedure.HasParameter(item => item.IsActive);
            procedure.HasOriginalValueParameter(item => item.IsActive, parameter => parameter.HasName("OriginalIsActive"));
            procedure.HasParameter(item => item.LastName);
            procedure.HasOriginalValueParameter(item => item.LastName, parameter => parameter.HasName("OriginalLastName"));
            procedure.HasParameter(item => item.MiddleName);
            procedure.HasOriginalValueParameter(item => item.MiddleName, parameter => parameter.HasName("OriginalMiddleName"));
            procedure.HasParameter(item => item.PasswordHash);
            procedure.HasOriginalValueParameter(item => item.PasswordHash, parameter => parameter.HasName("OriginalPasswordHash"));
            procedure.HasParameter(item => item.ProfileImagePath);
            procedure.HasOriginalValueParameter(item => item.ProfileImagePath, parameter => parameter.HasName("OriginalProfileImagePath"));
            procedure.HasParameter(item => item.Role);
            procedure.HasOriginalValueParameter(item => item.Role, parameter => parameter.HasName("OriginalRole"));
            procedure.HasRowsAffectedResultColumn(column => column.HasName("RowsAffected"));
        });

        var siteContent = modelBuilder.Entity<SiteContent>();
        siteContent.InsertUsingStoredProcedure("SiteContent_Insert", "dbo", procedure =>
        {
            procedure.HasParameter(item => item.Id);
            procedure.HasParameter(item => item.AboutDescription);
            procedure.HasParameter(item => item.AboutTitle);
            procedure.HasParameter(item => item.ContactBackgroundImagePath);
            procedure.HasParameter(item => item.ContactEmail);
            procedure.HasParameter(item => item.ContactPhone);
            procedure.HasParameter(item => item.FaviconPath);
            procedure.HasParameter(item => item.HeroDescription);
            procedure.HasParameter(item => item.HeroTitle);
            procedure.HasParameter(item => item.StudioName);
            procedure.HasParameter(item => item.Tagline);
        });
        siteContent.UpdateUsingStoredProcedure("SiteContent_Update", "dbo", procedure =>
        {
            procedure.HasOriginalValueParameter(item => item.Id, parameter => parameter.HasName("Id"));
            procedure.HasParameter(item => item.AboutDescription);
            procedure.HasOriginalValueParameter(item => item.AboutDescription, parameter => parameter.HasName("OriginalAboutDescription"));
            procedure.HasParameter(item => item.AboutTitle);
            procedure.HasOriginalValueParameter(item => item.AboutTitle, parameter => parameter.HasName("OriginalAboutTitle"));
            procedure.HasParameter(item => item.ContactBackgroundImagePath);
            procedure.HasOriginalValueParameter(item => item.ContactBackgroundImagePath, parameter => parameter.HasName("OriginalContactBackgroundImagePath"));
            procedure.HasParameter(item => item.ContactEmail);
            procedure.HasOriginalValueParameter(item => item.ContactEmail, parameter => parameter.HasName("OriginalContactEmail"));
            procedure.HasParameter(item => item.ContactPhone);
            procedure.HasOriginalValueParameter(item => item.ContactPhone, parameter => parameter.HasName("OriginalContactPhone"));
            procedure.HasParameter(item => item.FaviconPath);
            procedure.HasOriginalValueParameter(item => item.FaviconPath, parameter => parameter.HasName("OriginalFaviconPath"));
            procedure.HasParameter(item => item.HeroDescription);
            procedure.HasOriginalValueParameter(item => item.HeroDescription, parameter => parameter.HasName("OriginalHeroDescription"));
            procedure.HasParameter(item => item.HeroTitle);
            procedure.HasOriginalValueParameter(item => item.HeroTitle, parameter => parameter.HasName("OriginalHeroTitle"));
            procedure.HasParameter(item => item.StudioName);
            procedure.HasOriginalValueParameter(item => item.StudioName, parameter => parameter.HasName("OriginalStudioName"));
            procedure.HasParameter(item => item.Tagline);
            procedure.HasOriginalValueParameter(item => item.Tagline, parameter => parameter.HasName("OriginalTagline"));
            procedure.HasRowsAffectedResultColumn(column => column.HasName("RowsAffected"));
        });
        siteContent.DeleteUsingStoredProcedure("SiteContent_Delete", "dbo", procedure =>
        {
            procedure.HasOriginalValueParameter(item => item.Id, parameter => parameter.HasName("Id"));
            procedure.HasRowsAffectedResultColumn(column => column.HasName("RowsAffected"));
        });

        var projects = modelBuilder.Entity<Project>();
        projects.InsertUsingStoredProcedure("Projects_Insert", "dbo", procedure =>
        {
            procedure.HasParameter(item => item.Description);
            procedure.HasParameter(item => item.ImagePath);
            procedure.HasParameter(item => item.IsPublished);
            procedure.HasParameter(item => item.SortOrder);
            procedure.HasParameter(item => item.Title);
            procedure.HasParameter(item => item.WebsiteUrl);
            procedure.HasResultColumn(item => item.Id);
        });
        projects.UpdateUsingStoredProcedure("Projects_Update", "dbo", procedure =>
        {
            procedure.HasOriginalValueParameter(item => item.Id, parameter => parameter.HasName("Id"));
            procedure.HasParameter(item => item.Description);
            procedure.HasOriginalValueParameter(item => item.Description, parameter => parameter.HasName("OriginalDescription"));
            procedure.HasParameter(item => item.ImagePath);
            procedure.HasOriginalValueParameter(item => item.ImagePath, parameter => parameter.HasName("OriginalImagePath"));
            procedure.HasParameter(item => item.IsPublished);
            procedure.HasOriginalValueParameter(item => item.IsPublished, parameter => parameter.HasName("OriginalIsPublished"));
            procedure.HasParameter(item => item.SortOrder);
            procedure.HasOriginalValueParameter(item => item.SortOrder, parameter => parameter.HasName("OriginalSortOrder"));
            procedure.HasParameter(item => item.Title);
            procedure.HasOriginalValueParameter(item => item.Title, parameter => parameter.HasName("OriginalTitle"));
            procedure.HasParameter(item => item.WebsiteUrl);
            procedure.HasOriginalValueParameter(item => item.WebsiteUrl, parameter => parameter.HasName("OriginalWebsiteUrl"));
            procedure.HasRowsAffectedResultColumn(column => column.HasName("RowsAffected"));
        });
        projects.DeleteUsingStoredProcedure("Projects_Delete", "dbo", procedure =>
        {
            procedure.HasOriginalValueParameter(item => item.Id, parameter => parameter.HasName("Id"));
            procedure.HasRowsAffectedResultColumn(column => column.HasName("RowsAffected"));
        });

        var services = modelBuilder.Entity<ServiceOffering>();
        services.InsertUsingStoredProcedure("Services_Insert", "dbo", procedure =>
        {
            procedure.HasParameter(item => item.Description);
            procedure.HasParameter(item => item.ImagePath);
            procedure.HasParameter(item => item.IsPublished);
            procedure.HasParameter(item => item.SortOrder);
            procedure.HasParameter(item => item.Title);
            procedure.HasResultColumn(item => item.Id);
        });
        services.UpdateUsingStoredProcedure("Services_Update", "dbo", procedure =>
        {
            procedure.HasOriginalValueParameter(item => item.Id, parameter => parameter.HasName("Id"));
            procedure.HasParameter(item => item.Description);
            procedure.HasOriginalValueParameter(item => item.Description, parameter => parameter.HasName("OriginalDescription"));
            procedure.HasParameter(item => item.ImagePath);
            procedure.HasOriginalValueParameter(item => item.ImagePath, parameter => parameter.HasName("OriginalImagePath"));
            procedure.HasParameter(item => item.IsPublished);
            procedure.HasOriginalValueParameter(item => item.IsPublished, parameter => parameter.HasName("OriginalIsPublished"));
            procedure.HasParameter(item => item.SortOrder);
            procedure.HasOriginalValueParameter(item => item.SortOrder, parameter => parameter.HasName("OriginalSortOrder"));
            procedure.HasParameter(item => item.Title);
            procedure.HasOriginalValueParameter(item => item.Title, parameter => parameter.HasName("OriginalTitle"));
            procedure.HasRowsAffectedResultColumn(column => column.HasName("RowsAffected"));
        });
        services.DeleteUsingStoredProcedure("Services_Delete", "dbo", procedure =>
        {
            procedure.HasOriginalValueParameter(item => item.Id, parameter => parameter.HasName("Id"));
            procedure.HasRowsAffectedResultColumn(column => column.HasName("RowsAffected"));
        });

        var teamMembers = modelBuilder.Entity<TeamMember>();
        teamMembers.InsertUsingStoredProcedure("TeamMembers_Insert", "dbo", procedure =>
        {
            procedure.HasParameter(item => item.Bio);
            procedure.HasParameter(item => item.FirstName);
            procedure.HasParameter(item => item.IsPublished);
            procedure.HasParameter(item => item.LastName);
            procedure.HasParameter(item => item.MiddleName);
            procedure.HasParameter(item => item.PhotoPath);
            procedure.HasParameter(item => item.RoleTitle);
            procedure.HasParameter(item => item.SortOrder);
            procedure.HasResultColumn(item => item.Id);
        });
        teamMembers.UpdateUsingStoredProcedure("TeamMembers_Update", "dbo", procedure =>
        {
            procedure.HasOriginalValueParameter(item => item.Id, parameter => parameter.HasName("Id"));
            procedure.HasParameter(item => item.Bio);
            procedure.HasOriginalValueParameter(item => item.Bio, parameter => parameter.HasName("OriginalBio"));
            procedure.HasParameter(item => item.FirstName);
            procedure.HasOriginalValueParameter(item => item.FirstName, parameter => parameter.HasName("OriginalFirstName"));
            procedure.HasParameter(item => item.IsPublished);
            procedure.HasOriginalValueParameter(item => item.IsPublished, parameter => parameter.HasName("OriginalIsPublished"));
            procedure.HasParameter(item => item.LastName);
            procedure.HasOriginalValueParameter(item => item.LastName, parameter => parameter.HasName("OriginalLastName"));
            procedure.HasParameter(item => item.MiddleName);
            procedure.HasOriginalValueParameter(item => item.MiddleName, parameter => parameter.HasName("OriginalMiddleName"));
            procedure.HasParameter(item => item.PhotoPath);
            procedure.HasOriginalValueParameter(item => item.PhotoPath, parameter => parameter.HasName("OriginalPhotoPath"));
            procedure.HasParameter(item => item.RoleTitle);
            procedure.HasOriginalValueParameter(item => item.RoleTitle, parameter => parameter.HasName("OriginalRoleTitle"));
            procedure.HasParameter(item => item.SortOrder);
            procedure.HasOriginalValueParameter(item => item.SortOrder, parameter => parameter.HasName("OriginalSortOrder"));
            procedure.HasRowsAffectedResultColumn(column => column.HasName("RowsAffected"));
        });
        teamMembers.DeleteUsingStoredProcedure("TeamMembers_Delete", "dbo", procedure =>
        {
            procedure.HasOriginalValueParameter(item => item.Id, parameter => parameter.HasName("Id"));
            procedure.HasRowsAffectedResultColumn(column => column.HasName("RowsAffected"));
        });

        var socialLinks = modelBuilder.Entity<SocialLink>();
        socialLinks.InsertUsingStoredProcedure("SocialLinks_Insert", "dbo", procedure =>
        {
            procedure.HasParameter(item => item.CustomIconSvg);
            procedure.HasParameter(item => item.Label);
            procedure.HasParameter(item => item.SortOrder);
            procedure.HasParameter(item => item.Url);
            procedure.HasResultColumn(item => item.Id);
        });
        socialLinks.UpdateUsingStoredProcedure("SocialLinks_Update", "dbo", procedure =>
        {
            procedure.HasOriginalValueParameter(item => item.Id, parameter => parameter.HasName("Id"));
            procedure.HasParameter(item => item.CustomIconSvg);
            procedure.HasOriginalValueParameter(item => item.CustomIconSvg, parameter => parameter.HasName("OriginalCustomIconSvg"));
            procedure.HasParameter(item => item.Label);
            procedure.HasOriginalValueParameter(item => item.Label, parameter => parameter.HasName("OriginalLabel"));
            procedure.HasParameter(item => item.SortOrder);
            procedure.HasOriginalValueParameter(item => item.SortOrder, parameter => parameter.HasName("OriginalSortOrder"));
            procedure.HasParameter(item => item.Url);
            procedure.HasOriginalValueParameter(item => item.Url, parameter => parameter.HasName("OriginalUrl"));
            procedure.HasRowsAffectedResultColumn(column => column.HasName("RowsAffected"));
        });
        socialLinks.DeleteUsingStoredProcedure("SocialLinks_Delete", "dbo", procedure =>
        {
            procedure.HasOriginalValueParameter(item => item.Id, parameter => parameter.HasName("Id"));
            procedure.HasRowsAffectedResultColumn(column => column.HasName("RowsAffected"));
        });
    }
}
