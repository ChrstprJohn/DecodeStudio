-- Run on the local SQL Server as an administrator.
USE [master];
SET NOCOUNT ON;

IF DB_ID(N'portfolio') IS NULL
    THROW 50001, 'The source database portfolio is missing.', 1;
IF DB_ID(N'portfolio_school') IS NOT NULL
    THROW 50002, 'portfolio_school already exists. The copy was skipped.', 1;

DECLARE @suffix nvarchar(36) = CONVERT(nvarchar(36), NEWID());
DECLARE @backup nvarchar(4000) =
    CONVERT(nvarchar(4000), SERVERPROPERTY('InstanceDefaultBackupPath')) +
    N'portfolio-school-' + @suffix + N'.bak';
DECLARE @data nvarchar(4000) =
    CONVERT(nvarchar(4000), SERVERPROPERTY('InstanceDefaultDataPath')) +
    N'portfolio_school.mdf';
DECLARE @log nvarchar(4000) =
    CONVERT(nvarchar(4000), SERVERPROPERTY('InstanceDefaultLogPath')) +
    N'portfolio_school_log.ldf';

BACKUP DATABASE [portfolio] TO DISK = @backup WITH COPY_ONLY, CHECKSUM;
RESTORE DATABASE [portfolio_school] FROM DISK = @backup
    WITH MOVE N'portfolio' TO @data, MOVE N'portfolio_log' TO @log, CHECKSUM;

IF (SELECT COUNT(*) FROM portfolio_school.sys.tables WHERE is_ms_shipped = 0) <> 6
    THROW 50003, 'The school copy does not have the expected six tables.', 1;

SELECT N'Users' AS TableName, COUNT_BIG(*) AS Rows FROM portfolio_school.dbo.Users
UNION ALL SELECT N'SiteContent', COUNT_BIG(*) FROM portfolio_school.dbo.SiteContent
UNION ALL SELECT N'Projects', COUNT_BIG(*) FROM portfolio_school.dbo.Projects
UNION ALL SELECT N'Services', COUNT_BIG(*) FROM portfolio_school.dbo.Services
UNION ALL SELECT N'TeamMembers', COUNT_BIG(*) FROM portfolio_school.dbo.TeamMembers
UNION ALL SELECT N'SocialLinks', COUNT_BIG(*) FROM portfolio_school.dbo.SocialLinks;
