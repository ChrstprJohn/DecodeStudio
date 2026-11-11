-- Run after schema.sql. Pass PORTFOLIO_DB_PASSWORD to sqlcmd via its environment.
-- The deployment generates a password from a fixed complexity prefix and random hex.
USE [portfolio];
GO

IF SUSER_ID(N'portfolio_app') IS NULL
    CREATE LOGIN [portfolio_app] WITH PASSWORD = '$(PORTFOLIO_DB_PASSWORD)', DEFAULT_DATABASE = [portfolio];

IF DATABASE_PRINCIPAL_ID(N'portfolio_app') IS NULL
    CREATE USER [portfolio_app] FOR LOGIN [portfolio_app];

GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO [portfolio_app];
GO
