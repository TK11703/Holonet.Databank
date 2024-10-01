CREATE TABLE [dbo].[HistoricalEventCharacters]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[CharacterId] INT NOT NULL,
	[HistoricalEventId] INT NOT NULL,
	[CreatedOn] DATETIME NOT NULL,
	[CreatedBy] NVARCHAR(250) NOT NULL,
	CONSTRAINT [FK_HistoricalEventCharacters_Characters] FOREIGN KEY ([CharacterId]) REFERENCES [Characters]([Id]),
	CONSTRAINT [FK_HistoricalEventCharacters_HistoricalEvents] FOREIGN KEY ([HistoricalEventId]) REFERENCES [HistoricalEvents]([Id]),
)
