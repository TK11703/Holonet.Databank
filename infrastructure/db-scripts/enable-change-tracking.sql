-- 🧾 Enable Change Tracking on the database if not already enabled
IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_databases WHERE database_id = DB_ID())
BEGIN
    PRINT 'Enabling CHANGE_TRACKING on the database...';
    
    ALTER DATABASE CURRENT SET CHANGE_TRACKING = ON (CHANGE_RETENTION = 2 DAYS, AUTO_CLEANUP = ON);
END
ELSE
BEGIN
    PRINT 'CHANGE_TRACKING is already enabled on the database.';
END
  
-- 🧾 Enable Change Tracking on dbo.DataRecords if not already enabled
IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_tables WHERE object_id = OBJECT_ID('dbo.DataRecords'))
BEGIN
    PRINT 'Enabling CHANGE_TRACKING on dbo.DataRecords...';
    
    ALTER TABLE [dbo].[DataRecords] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);
END
ELSE
BEGIN
    PRINT 'CHANGE_TRACKING is already enabled on dbo.DataRecords.';
END
