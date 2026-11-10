USE portfolio;
IF COL_LENGTH('dbo.Services', 'ImagePath') IS NULL
    ALTER TABLE dbo.Services ADD ImagePath nvarchar(2048) NULL;
