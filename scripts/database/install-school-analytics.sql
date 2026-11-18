USE [portfolio_school];
GO

CREATE OR ALTER PROCEDURE dbo.Analytics_ReadTotals
AS
BEGIN
    SET NOCOUNT ON;

    SELECT accounts.ActiveAccounts, accounts.InactiveAccounts,
           projects.PublishedProjects, projects.DraftProjects,
           services.PublishedServices, services.DraftServices,
           team.PublishedTeamMembers, team.DraftTeamMembers
    FROM (SELECT COUNT(CASE WHEN IsActive = 1 THEN 1 END) AS ActiveAccounts,
                 COUNT(CASE WHEN IsActive = 0 THEN 1 END) AS InactiveAccounts FROM dbo.Users) AS accounts
    CROSS JOIN (SELECT COUNT(CASE WHEN IsPublished = 1 THEN 1 END) AS PublishedProjects,
                       COUNT(CASE WHEN IsPublished = 0 THEN 1 END) AS DraftProjects FROM dbo.Projects) AS projects
    CROSS JOIN (SELECT COUNT(CASE WHEN IsPublished = 1 THEN 1 END) AS PublishedServices,
                       COUNT(CASE WHEN IsPublished = 0 THEN 1 END) AS DraftServices FROM dbo.Services) AS services
    CROSS JOIN (SELECT COUNT(CASE WHEN IsPublished = 1 THEN 1 END) AS PublishedTeamMembers,
                       COUNT(CASE WHEN IsPublished = 0 THEN 1 END) AS DraftTeamMembers FROM dbo.TeamMembers) AS team;
END;
GO

CREATE OR ALTER PROCEDURE dbo.Analytics_ReadRegistrations
    @StartUtc datetime2(7),
    @EndUtc datetime2(7)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CONVERT(date, DATEADD(hour, 8, CreatedAtUtc)) AS [Date], COUNT(*) AS Registrations
    FROM dbo.Users
    WHERE CreatedAtUtc >= @StartUtc AND CreatedAtUtc < @EndUtc
    GROUP BY CONVERT(date, DATEADD(hour, 8, CreatedAtUtc))
    ORDER BY [Date];
END;
GO
