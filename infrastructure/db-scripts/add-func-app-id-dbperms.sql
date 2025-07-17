-- Declare Function App Identity as a variable
DECLARE @FunctionAppIdentity NVARCHAR(128) = 'Holonet-Func-Databank-8108';

-- Add identity to db_datareader role
EXEC sp_addrolemember 'db_datareader', @FunctionAppIdentity;

-- Grant VIEW DATABASE STATE
EXEC('GRANT VIEW DATABASE STATE TO [' + @FunctionAppIdentity + ']');

-- Grant VIEW CHANGE TRACKING on the target table
DECLARE @TableName NVARCHAR(128) = 'dbo.DataRecords';
EXEC('GRANT VIEW CHANGE TRACKING ON ' + @TableName + ' TO [' + @FunctionAppIdentity + ']');
