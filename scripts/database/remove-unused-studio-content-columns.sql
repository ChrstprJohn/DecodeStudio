SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF COL_LENGTH('dbo.SiteContent', 'HeroImagePath') IS NOT NULL
    ALTER TABLE dbo.SiteContent DROP COLUMN HeroImagePath;
IF COL_LENGTH('dbo.SiteContent', 'HeroLinkText') IS NOT NULL
    ALTER TABLE dbo.SiteContent DROP COLUMN HeroLinkText;
IF COL_LENGTH('dbo.SiteContent', 'HeroLinkUrl') IS NOT NULL
    ALTER TABLE dbo.SiteContent DROP COLUMN HeroLinkUrl;
IF COL_LENGTH('dbo.SiteContent', 'FooterText') IS NOT NULL
    ALTER TABLE dbo.SiteContent DROP COLUMN FooterText;

COMMIT TRANSACTION;
