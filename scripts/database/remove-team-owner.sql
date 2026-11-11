IF COL_LENGTH('dbo.TeamMembers', 'IsOwner') IS NOT NULL
    ALTER TABLE dbo.TeamMembers DROP COLUMN IsOwner;
