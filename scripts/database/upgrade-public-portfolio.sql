-- Run on the VM against an existing portfolio database before deploying.
-- Back up the database first. The legacy fields dropped below are retired.
-- Select the portfolio database in your SQL editor before running this script.
IF DB_NAME() <> N'portfolio'
    THROW 50001, 'Select the portfolio database before running this upgrade.', 1;

SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF COL_LENGTH('dbo.SiteContent', 'ContactBackgroundImagePath') IS NULL
    ALTER TABLE dbo.SiteContent ADD ContactBackgroundImagePath nvarchar(500) NULL;
IF COL_LENGTH('dbo.Services', 'ImagePath') IS NULL
    ALTER TABLE dbo.Services ADD ImagePath nvarchar(2048) NULL;
IF COL_LENGTH('dbo.SocialLinks', 'CustomIconSvg') IS NULL
    ALTER TABLE dbo.SocialLinks ADD CustomIconSvg nvarchar(max) NULL;

IF COL_LENGTH('dbo.SiteContent', 'HeroImagePath') IS NOT NULL
    ALTER TABLE dbo.SiteContent DROP COLUMN HeroImagePath;
IF COL_LENGTH('dbo.SiteContent', 'HeroLinkText') IS NOT NULL
    ALTER TABLE dbo.SiteContent DROP COLUMN HeroLinkText;
IF COL_LENGTH('dbo.SiteContent', 'HeroLinkUrl') IS NOT NULL
    ALTER TABLE dbo.SiteContent DROP COLUMN HeroLinkUrl;
IF COL_LENGTH('dbo.SiteContent', 'FooterText') IS NOT NULL
    ALTER TABLE dbo.SiteContent DROP COLUMN FooterText;
IF COL_LENGTH('dbo.TeamMembers', 'IsOwner') IS NOT NULL
    ALTER TABLE dbo.TeamMembers DROP COLUMN IsOwner;

COMMIT TRANSACTION;
