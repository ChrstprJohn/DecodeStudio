-- Install after the school schema and any column upgrades.
USE [portfolio_school];
GO

CREATE OR ALTER PROCEDURE dbo.Users_Read
    @Id int = NULL, @Email nvarchar(256) = NULL, @IsActive bit = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, FirstName, MiddleName, LastName, Email, PasswordHash, ProfileImagePath, Role, IsActive, CreatedAtUtc
    FROM dbo.Users
    WHERE (@Id IS NULL OR Id = @Id) AND (@Email IS NULL OR Email = @Email)
        AND (@IsActive IS NULL OR IsActive = @IsActive);
END;
GO

CREATE OR ALTER PROCEDURE dbo.Users_ListAccounts
    @Id int = NULL,
    @IsActive bit = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, FirstName, MiddleName, LastName, Email, ProfileImagePath, Role, IsActive, CreatedAtUtc
    FROM dbo.Users
    WHERE (@Id IS NULL OR Id = @Id) AND (@IsActive IS NULL OR IsActive = @IsActive)
    ORDER BY CreatedAtUtc DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Users_FindEmail
    @Email nvarchar(256),
    @ExcludedId int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id AS [Value]
    FROM dbo.Users
    WHERE Email = @Email AND (@ExcludedId IS NULL OR Id <> @ExcludedId);
END;
GO

CREATE OR ALTER PROCEDURE dbo.Users_Insert
    @CreatedAtUtc datetime2(7),
    @Email nvarchar(256),
    @FirstName nvarchar(100),
    @IsActive bit,
    @LastName nvarchar(100),
    @MiddleName nvarchar(100),
    @PasswordHash nvarchar(500),
    @ProfileImagePath nvarchar(500),
    @Role nvarchar(20)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Users (CreatedAtUtc, Email, FirstName, IsActive, LastName, MiddleName, PasswordHash, ProfileImagePath, Role)
    VALUES (@CreatedAtUtc, @Email, @FirstName, @IsActive, @LastName, @MiddleName, @PasswordHash, @ProfileImagePath, @Role);
    SELECT CONVERT(int, SCOPE_IDENTITY()) AS Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Users_Update
    @Id int,
    @CreatedAtUtc datetime2(7),
    @OriginalCreatedAtUtc datetime2(7),
    @Email nvarchar(256),
    @OriginalEmail nvarchar(256),
    @FirstName nvarchar(100),
    @OriginalFirstName nvarchar(100),
    @IsActive bit,
    @OriginalIsActive bit,
    @LastName nvarchar(100),
    @OriginalLastName nvarchar(100),
    @MiddleName nvarchar(100),
    @OriginalMiddleName nvarchar(100),
    @PasswordHash nvarchar(500),
    @OriginalPasswordHash nvarchar(500),
    @ProfileImagePath nvarchar(500),
    @OriginalProfileImagePath nvarchar(500),
    @Role nvarchar(20),
    @OriginalRole nvarchar(20)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Users
    SET
        CreatedAtUtc = CASE WHEN @CreatedAtUtc = @OriginalCreatedAtUtc
            THEN CreatedAtUtc ELSE @CreatedAtUtc END,
        Email = CASE WHEN CONVERT(varbinary(max), @Email) = CONVERT(varbinary(max), @OriginalEmail)
            THEN Email ELSE @Email END,
        FirstName = CASE WHEN CONVERT(varbinary(max), @FirstName) = CONVERT(varbinary(max), @OriginalFirstName)
            THEN FirstName ELSE @FirstName END,
        IsActive = CASE WHEN @IsActive = @OriginalIsActive
            THEN IsActive ELSE @IsActive END,
        LastName = CASE WHEN CONVERT(varbinary(max), @LastName) = CONVERT(varbinary(max), @OriginalLastName)
            THEN LastName ELSE @LastName END,
        MiddleName = CASE WHEN CONVERT(varbinary(max), @MiddleName) = CONVERT(varbinary(max), @OriginalMiddleName) OR (@MiddleName IS NULL AND @OriginalMiddleName IS NULL)
            THEN MiddleName ELSE @MiddleName END,
        PasswordHash = CASE WHEN CONVERT(varbinary(max), @PasswordHash) = CONVERT(varbinary(max), @OriginalPasswordHash)
            THEN PasswordHash ELSE @PasswordHash END,
        ProfileImagePath = CASE WHEN CONVERT(varbinary(max), @ProfileImagePath) = CONVERT(varbinary(max), @OriginalProfileImagePath) OR (@ProfileImagePath IS NULL AND @OriginalProfileImagePath IS NULL)
            THEN ProfileImagePath ELSE @ProfileImagePath END,
        Role = CASE WHEN CONVERT(varbinary(max), @Role) = CONVERT(varbinary(max), @OriginalRole)
            THEN Role ELSE @Role END
    WHERE Id = @Id;
    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SiteContent_Read
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, StudioName, Tagline, HeroTitle, HeroDescription, ContactBackgroundImagePath, FaviconPath, AboutTitle, AboutDescription, ContactEmail, ContactPhone
    FROM dbo.SiteContent
    WHERE Id = 1;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SiteContent_Insert
    @Id int,
    @AboutDescription nvarchar(max),
    @AboutTitle nvarchar(200),
    @ContactBackgroundImagePath nvarchar(500),
    @ContactEmail nvarchar(256),
    @ContactPhone nvarchar(30),
    @FaviconPath nvarchar(500),
    @HeroDescription nvarchar(2000),
    @HeroTitle nvarchar(200),
    @StudioName nvarchar(150),
    @Tagline nvarchar(250)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.SiteContent (Id, AboutDescription, AboutTitle, ContactBackgroundImagePath, ContactEmail, ContactPhone, FaviconPath, HeroDescription, HeroTitle, StudioName, Tagline)
    VALUES (@Id, @AboutDescription, @AboutTitle, @ContactBackgroundImagePath, @ContactEmail, @ContactPhone, @FaviconPath, @HeroDescription, @HeroTitle, @StudioName, @Tagline);
END;
GO

CREATE OR ALTER PROCEDURE dbo.SiteContent_Update
    @Id int,
    @AboutDescription nvarchar(max),
    @OriginalAboutDescription nvarchar(max),
    @AboutTitle nvarchar(200),
    @OriginalAboutTitle nvarchar(200),
    @ContactBackgroundImagePath nvarchar(500),
    @OriginalContactBackgroundImagePath nvarchar(500),
    @ContactEmail nvarchar(256),
    @OriginalContactEmail nvarchar(256),
    @ContactPhone nvarchar(30),
    @OriginalContactPhone nvarchar(30),
    @FaviconPath nvarchar(500),
    @OriginalFaviconPath nvarchar(500),
    @HeroDescription nvarchar(2000),
    @OriginalHeroDescription nvarchar(2000),
    @HeroTitle nvarchar(200),
    @OriginalHeroTitle nvarchar(200),
    @StudioName nvarchar(150),
    @OriginalStudioName nvarchar(150),
    @Tagline nvarchar(250),
    @OriginalTagline nvarchar(250)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.SiteContent
    SET
        AboutDescription = CASE WHEN CONVERT(varbinary(max), @AboutDescription) = CONVERT(varbinary(max), @OriginalAboutDescription)
            THEN AboutDescription ELSE @AboutDescription END,
        AboutTitle = CASE WHEN CONVERT(varbinary(max), @AboutTitle) = CONVERT(varbinary(max), @OriginalAboutTitle)
            THEN AboutTitle ELSE @AboutTitle END,
        ContactBackgroundImagePath = CASE WHEN CONVERT(varbinary(max), @ContactBackgroundImagePath) = CONVERT(varbinary(max), @OriginalContactBackgroundImagePath) OR (@ContactBackgroundImagePath IS NULL AND @OriginalContactBackgroundImagePath IS NULL)
            THEN ContactBackgroundImagePath ELSE @ContactBackgroundImagePath END,
        ContactEmail = CASE WHEN CONVERT(varbinary(max), @ContactEmail) = CONVERT(varbinary(max), @OriginalContactEmail)
            THEN ContactEmail ELSE @ContactEmail END,
        ContactPhone = CASE WHEN CONVERT(varbinary(max), @ContactPhone) = CONVERT(varbinary(max), @OriginalContactPhone) OR (@ContactPhone IS NULL AND @OriginalContactPhone IS NULL)
            THEN ContactPhone ELSE @ContactPhone END,
        FaviconPath = CASE WHEN CONVERT(varbinary(max), @FaviconPath) = CONVERT(varbinary(max), @OriginalFaviconPath) OR (@FaviconPath IS NULL AND @OriginalFaviconPath IS NULL)
            THEN FaviconPath ELSE @FaviconPath END,
        HeroDescription = CASE WHEN CONVERT(varbinary(max), @HeroDescription) = CONVERT(varbinary(max), @OriginalHeroDescription)
            THEN HeroDescription ELSE @HeroDescription END,
        HeroTitle = CASE WHEN CONVERT(varbinary(max), @HeroTitle) = CONVERT(varbinary(max), @OriginalHeroTitle)
            THEN HeroTitle ELSE @HeroTitle END,
        StudioName = CASE WHEN CONVERT(varbinary(max), @StudioName) = CONVERT(varbinary(max), @OriginalStudioName)
            THEN StudioName ELSE @StudioName END,
        Tagline = CASE WHEN CONVERT(varbinary(max), @Tagline) = CONVERT(varbinary(max), @OriginalTagline) OR (@Tagline IS NULL AND @OriginalTagline IS NULL)
            THEN Tagline ELSE @Tagline END
    WHERE Id = @Id;
    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SiteContent_Delete
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.SiteContent WHERE Id = @Id;
    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Projects_Read
    @Id int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Title, Description, ImagePath, WebsiteUrl, SortOrder, IsPublished
    FROM dbo.Projects
    WHERE @Id IS NULL OR Id = @Id
    ORDER BY SortOrder, Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Projects_Insert
    @Description nvarchar(2000),
    @ImagePath nvarchar(500),
    @IsPublished bit,
    @SortOrder int,
    @Title nvarchar(200),
    @WebsiteUrl nvarchar(2048)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Projects (Description, ImagePath, IsPublished, SortOrder, Title, WebsiteUrl)
    VALUES (@Description, @ImagePath, @IsPublished, @SortOrder, @Title, @WebsiteUrl);
    SELECT CONVERT(int, SCOPE_IDENTITY()) AS Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Projects_Update
    @Id int,
    @Description nvarchar(2000),
    @OriginalDescription nvarchar(2000),
    @ImagePath nvarchar(500),
    @OriginalImagePath nvarchar(500),
    @IsPublished bit,
    @OriginalIsPublished bit,
    @SortOrder int,
    @OriginalSortOrder int,
    @Title nvarchar(200),
    @OriginalTitle nvarchar(200),
    @WebsiteUrl nvarchar(2048),
    @OriginalWebsiteUrl nvarchar(2048)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Projects
    SET
        Description = CASE WHEN CONVERT(varbinary(max), @Description) = CONVERT(varbinary(max), @OriginalDescription)
            THEN Description ELSE @Description END,
        ImagePath = CASE WHEN CONVERT(varbinary(max), @ImagePath) = CONVERT(varbinary(max), @OriginalImagePath)
            THEN ImagePath ELSE @ImagePath END,
        IsPublished = CASE WHEN @IsPublished = @OriginalIsPublished
            THEN IsPublished ELSE @IsPublished END,
        SortOrder = CASE WHEN @SortOrder = @OriginalSortOrder
            THEN SortOrder ELSE @SortOrder END,
        Title = CASE WHEN CONVERT(varbinary(max), @Title) = CONVERT(varbinary(max), @OriginalTitle)
            THEN Title ELSE @Title END,
        WebsiteUrl = CASE WHEN CONVERT(varbinary(max), @WebsiteUrl) = CONVERT(varbinary(max), @OriginalWebsiteUrl)
            THEN WebsiteUrl ELSE @WebsiteUrl END
    WHERE Id = @Id;
    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Projects_Delete
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Projects WHERE Id = @Id;
    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Services_Read
    @Id int = NULL, @PublishedOnly bit = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Title, Description, ImagePath, SortOrder, IsPublished
    FROM dbo.Services
    WHERE (@Id IS NULL OR Id = @Id) AND (@PublishedOnly = 0 OR IsPublished = 1)
    ORDER BY SortOrder, Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Services_Insert
    @Description nvarchar(2000),
    @ImagePath nvarchar(2048),
    @IsPublished bit,
    @SortOrder int,
    @Title nvarchar(200)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Services (Description, ImagePath, IsPublished, SortOrder, Title)
    VALUES (@Description, @ImagePath, @IsPublished, @SortOrder, @Title);
    SELECT CONVERT(int, SCOPE_IDENTITY()) AS Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Services_Update
    @Id int,
    @Description nvarchar(2000),
    @OriginalDescription nvarchar(2000),
    @ImagePath nvarchar(2048),
    @OriginalImagePath nvarchar(2048),
    @IsPublished bit,
    @OriginalIsPublished bit,
    @SortOrder int,
    @OriginalSortOrder int,
    @Title nvarchar(200),
    @OriginalTitle nvarchar(200)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Services
    SET
        Description = CASE WHEN CONVERT(varbinary(max), @Description) = CONVERT(varbinary(max), @OriginalDescription)
            THEN Description ELSE @Description END,
        ImagePath = CASE WHEN CONVERT(varbinary(max), @ImagePath) = CONVERT(varbinary(max), @OriginalImagePath) OR (@ImagePath IS NULL AND @OriginalImagePath IS NULL)
            THEN ImagePath ELSE @ImagePath END,
        IsPublished = CASE WHEN @IsPublished = @OriginalIsPublished
            THEN IsPublished ELSE @IsPublished END,
        SortOrder = CASE WHEN @SortOrder = @OriginalSortOrder
            THEN SortOrder ELSE @SortOrder END,
        Title = CASE WHEN CONVERT(varbinary(max), @Title) = CONVERT(varbinary(max), @OriginalTitle)
            THEN Title ELSE @Title END
    WHERE Id = @Id;
    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Services_Delete
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Services WHERE Id = @Id;
    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO

CREATE OR ALTER PROCEDURE dbo.TeamMembers_Read
    @Id int = NULL, @PublishedOnly bit = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, FirstName, MiddleName, LastName, RoleTitle, Bio, PhotoPath, SortOrder, IsPublished
    FROM dbo.TeamMembers
    WHERE (@Id IS NULL OR Id = @Id) AND (@PublishedOnly = 0 OR IsPublished = 1)
    ORDER BY SortOrder, Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.TeamMembers_Insert
    @Bio nvarchar(2000),
    @FirstName nvarchar(100),
    @IsPublished bit,
    @LastName nvarchar(100),
    @MiddleName nvarchar(100),
    @PhotoPath nvarchar(500),
    @RoleTitle nvarchar(150),
    @SortOrder int
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.TeamMembers (Bio, FirstName, IsPublished, LastName, MiddleName, PhotoPath, RoleTitle, SortOrder)
    VALUES (@Bio, @FirstName, @IsPublished, @LastName, @MiddleName, @PhotoPath, @RoleTitle, @SortOrder);
    SELECT CONVERT(int, SCOPE_IDENTITY()) AS Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.TeamMembers_Update
    @Id int,
    @Bio nvarchar(2000),
    @OriginalBio nvarchar(2000),
    @FirstName nvarchar(100),
    @OriginalFirstName nvarchar(100),
    @IsPublished bit,
    @OriginalIsPublished bit,
    @LastName nvarchar(100),
    @OriginalLastName nvarchar(100),
    @MiddleName nvarchar(100),
    @OriginalMiddleName nvarchar(100),
    @PhotoPath nvarchar(500),
    @OriginalPhotoPath nvarchar(500),
    @RoleTitle nvarchar(150),
    @OriginalRoleTitle nvarchar(150),
    @SortOrder int,
    @OriginalSortOrder int
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.TeamMembers
    SET
        Bio = CASE WHEN CONVERT(varbinary(max), @Bio) = CONVERT(varbinary(max), @OriginalBio)
            THEN Bio ELSE @Bio END,
        FirstName = CASE WHEN CONVERT(varbinary(max), @FirstName) = CONVERT(varbinary(max), @OriginalFirstName)
            THEN FirstName ELSE @FirstName END,
        IsPublished = CASE WHEN @IsPublished = @OriginalIsPublished
            THEN IsPublished ELSE @IsPublished END,
        LastName = CASE WHEN CONVERT(varbinary(max), @LastName) = CONVERT(varbinary(max), @OriginalLastName)
            THEN LastName ELSE @LastName END,
        MiddleName = CASE WHEN CONVERT(varbinary(max), @MiddleName) = CONVERT(varbinary(max), @OriginalMiddleName) OR (@MiddleName IS NULL AND @OriginalMiddleName IS NULL)
            THEN MiddleName ELSE @MiddleName END,
        PhotoPath = CASE WHEN CONVERT(varbinary(max), @PhotoPath) = CONVERT(varbinary(max), @OriginalPhotoPath) OR (@PhotoPath IS NULL AND @OriginalPhotoPath IS NULL)
            THEN PhotoPath ELSE @PhotoPath END,
        RoleTitle = CASE WHEN CONVERT(varbinary(max), @RoleTitle) = CONVERT(varbinary(max), @OriginalRoleTitle)
            THEN RoleTitle ELSE @RoleTitle END,
        SortOrder = CASE WHEN @SortOrder = @OriginalSortOrder
            THEN SortOrder ELSE @SortOrder END
    WHERE Id = @Id;
    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO

CREATE OR ALTER PROCEDURE dbo.TeamMembers_Delete
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.TeamMembers WHERE Id = @Id;
    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SocialLinks_Read
    @Id int = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Label, Url, SortOrder, CustomIconSvg
    FROM dbo.SocialLinks
    WHERE @Id IS NULL OR Id = @Id
    ORDER BY SortOrder, Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SocialLinks_Insert
    @CustomIconSvg nvarchar(max),
    @Label nvarchar(100),
    @SortOrder int,
    @Url nvarchar(2048)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.SocialLinks (CustomIconSvg, Label, SortOrder, Url)
    VALUES (@CustomIconSvg, @Label, @SortOrder, @Url);
    SELECT CONVERT(int, SCOPE_IDENTITY()) AS Id;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SocialLinks_Update
    @Id int,
    @CustomIconSvg nvarchar(max),
    @OriginalCustomIconSvg nvarchar(max),
    @Label nvarchar(100),
    @OriginalLabel nvarchar(100),
    @SortOrder int,
    @OriginalSortOrder int,
    @Url nvarchar(2048),
    @OriginalUrl nvarchar(2048)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.SocialLinks
    SET
        CustomIconSvg = CASE WHEN CONVERT(varbinary(max), @CustomIconSvg) = CONVERT(varbinary(max), @OriginalCustomIconSvg) OR (@CustomIconSvg IS NULL AND @OriginalCustomIconSvg IS NULL)
            THEN CustomIconSvg ELSE @CustomIconSvg END,
        Label = CASE WHEN CONVERT(varbinary(max), @Label) = CONVERT(varbinary(max), @OriginalLabel)
            THEN Label ELSE @Label END,
        SortOrder = CASE WHEN @SortOrder = @OriginalSortOrder
            THEN SortOrder ELSE @SortOrder END,
        Url = CASE WHEN CONVERT(varbinary(max), @Url) = CONVERT(varbinary(max), @OriginalUrl)
            THEN Url ELSE @Url END
    WHERE Id = @Id;
    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO

CREATE OR ALTER PROCEDURE dbo.SocialLinks_Delete
    @Id int
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.SocialLinks WHERE Id = @Id;
    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO
