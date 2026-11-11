-- Decode Studio: SQL Server table schema.
-- Creates portfolio if needed, then creates its tables. No sample data.
-- Run the whole script in a SQL Server editor supporting GO (e.g. SSMS).
-- Run table creation once; existing tables are not dropped or overwritten.

USE master;
GO

IF DB_ID(N'portfolio') IS NULL
    EXEC(N'CREATE DATABASE [portfolio]');
GO

USE [portfolio];
GO

CREATE TABLE dbo.Users (
    Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
    FirstName nvarchar(100) NOT NULL,
    MiddleName nvarchar(100) NULL,
    LastName nvarchar(100) NOT NULL,
    Email nvarchar(256) NOT NULL CONSTRAINT UQ_Users_Email UNIQUE,
    PasswordHash nvarchar(500) NOT NULL,
    ProfileImagePath nvarchar(500) NULL,
    Role nvarchar(20) NOT NULL,
    IsActive bit NOT NULL,
    CreatedAtUtc datetime2(7) NOT NULL
);

CREATE TABLE dbo.SiteContent (
    Id int NOT NULL CONSTRAINT PK_SiteContent PRIMARY KEY,
    StudioName nvarchar(150) NOT NULL,
    Tagline nvarchar(250) NULL,
    HeroTitle nvarchar(200) NOT NULL,
    HeroDescription nvarchar(2000) NOT NULL,
    ContactBackgroundImagePath nvarchar(500) NULL,
    AboutTitle nvarchar(200) NOT NULL,
    AboutDescription nvarchar(max) NOT NULL,
    ContactEmail nvarchar(256) NOT NULL,
    ContactPhone nvarchar(30) NULL
);

CREATE TABLE dbo.Projects (
    Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Projects PRIMARY KEY,
    Title nvarchar(200) NOT NULL,
    Description nvarchar(2000) NOT NULL,
    ImagePath nvarchar(500) NOT NULL,
    WebsiteUrl nvarchar(2048) NOT NULL,
    SortOrder int NOT NULL,
    IsPublished bit NOT NULL
);

CREATE TABLE dbo.Services (
    Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Services PRIMARY KEY,
    Title nvarchar(200) NOT NULL,
    Description nvarchar(2000) NOT NULL,
    ImagePath nvarchar(2048) NULL,
    SortOrder int NOT NULL,
    IsPublished bit NOT NULL
);

CREATE TABLE dbo.TeamMembers (
    Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_TeamMembers PRIMARY KEY,
    FirstName nvarchar(100) NOT NULL,
    MiddleName nvarchar(100) NULL,
    LastName nvarchar(100) NOT NULL,
    RoleTitle nvarchar(150) NOT NULL,
    Bio nvarchar(2000) NOT NULL,
    PhotoPath nvarchar(500) NULL,
    SortOrder int NOT NULL,
    IsPublished bit NOT NULL
);

CREATE TABLE dbo.SocialLinks (
    Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_SocialLinks PRIMARY KEY,
    Label nvarchar(100) NOT NULL,
    Url nvarchar(2048) NOT NULL,
    SortOrder int NOT NULL,
    CustomIconSvg nvarchar(max) NULL
);

