/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

-- Disable change tracking for the current tables
IF EXISTS (SELECT 1 FROM sys.change_tracking_tables)
BEGIN
    ALTER TABLE [dbo].[DataRecords] DISABLE CHANGE_TRACKING;
END

-- Disable change tracking for the current database
IF EXISTS (SELECT 1 FROM sys.change_tracking_databases WHERE database_id = DB_ID())
BEGIN
    ALTER DATABASE CURRENT SET CHANGE_TRACKING = OFF WITH ROLLBACK IMMEDIATE;
END

