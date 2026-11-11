IF COL_LENGTH('dbo.SiteContent', 'FaviconPath') IS NULL
    ALTER TABLE dbo.SiteContent ADD FaviconPath nvarchar(500) NULL;
