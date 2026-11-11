IF COL_LENGTH('dbo.SiteContent', 'ContactBackgroundImagePath') IS NULL
    ALTER TABLE dbo.SiteContent ADD ContactBackgroundImagePath nvarchar(500) NULL;
