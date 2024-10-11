CREATE TABLE [dbo].[HistoricalEvents]
(
	[Id] INT NOT NULL PRIMARY KEY identity,
	[Name] NVARCHAR(150) NOT NULL,
	[Description] NVARCHAR(MAX) NULL,
	[DatePeriod] NVARCHAR(200) NULL,
	[Shard] NVARCHAR(500) NULL,
	[UpdatedOn] DATETIME NOT NULL, 
	[AuthorId] INT NOT NULL, 
	[Active] BIT NOT NULL Default 1,
	CONSTRAINT [FK_HistoricalEvents_Authors] FOREIGN KEY ([AuthorId]) REFERENCES [Authors]([Id]),
)

GO
CREATE INDEX [idx_HistoricalEvents_Name] ON [dbo].[HistoricalEvents]([Name]);
GO
