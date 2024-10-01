CREATE TABLE [dbo].[HistoricalEvents]
(
	[Id] INT NOT NULL PRIMARY KEY identity,
	[Name] NVARCHAR(150) NOT NULL,
	[Description] NVARCHAR(MAX) NULL,
	[DatePeriod] NVARCHAR(200) NULL,
	[Shard] NVARCHAR(500) NULL,
	[CreatedOn] DATETIME NOT NULL, 
	[CreatedBy] NVARCHAR(250) NOT NULL, 
	[UpdatedOn] DATETIME NOT NULL, 
	[UpdatedBy] NVARCHAR(250) NOT NULL, 
	[Active] BIT NOT NULL Default 1,
)
