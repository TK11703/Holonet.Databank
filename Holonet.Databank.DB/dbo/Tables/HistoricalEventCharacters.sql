CREATE TABLE [dbo].[HistoricalEventCharacters]
(
	[Id] INT NOT NULL PRIMARY KEY identity,
	[CharacterId] INT NOT NULL,
	[HistoricalEventId] INT NOT NULL,
	[UpdatedOn] DATETIME NOT NULL, 
	[AuthorId] INT NOT NULL, 
	CONSTRAINT [FK_HistoricalEventCharacters_Characters] FOREIGN KEY ([CharacterId]) REFERENCES [Characters]([Id]),
	CONSTRAINT [FK_HistoricalEventCharacters_HistoricalEvents] FOREIGN KEY ([HistoricalEventId]) REFERENCES [HistoricalEvents]([Id]),
	CONSTRAINT [FK_HistoricalEventCharacters_Authors] FOREIGN KEY ([AuthorId]) REFERENCES [Authors]([Id]),
)
