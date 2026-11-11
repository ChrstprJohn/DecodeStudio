IF COL_LENGTH('dbo.SocialLinks', 'CustomIconSvg') IS NULL
    ALTER TABLE dbo.SocialLinks ADD CustomIconSvg nvarchar(max) NULL;
