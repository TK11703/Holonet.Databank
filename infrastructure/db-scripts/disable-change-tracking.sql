DECLARE @sql NVARCHAR(MAX) = N'';

SELECT @sql += 'ALTER TABLE [' + s.name + '].[' + t.name + '] DISABLE CHANGE_TRACKING;' + CHAR(13)
FROM sys.change_tracking_tables ct
JOIN sys.tables t ON ct.object_id = t.object_id
JOIN sys.schemas s ON t.schema_id = s.schema_id;

EXEC sp_executesql @sql;

-- Disable change tracking for the current database
IF EXISTS (SELECT 1 FROM sys.change_tracking_databases WHERE database_id = DB_ID())
BEGIN
    ALTER DATABASE CURRENT SET CHANGE_TRACKING = OFF WITH ROLLBACK IMMEDIATE;
END
