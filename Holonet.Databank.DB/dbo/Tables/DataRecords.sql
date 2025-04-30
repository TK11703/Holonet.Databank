CREATE TABLE [dbo].[DataRecords]
(
	[Id] INT NOT NULL PRIMARY KEY identity,
	[Shard] NVARCHAR(500) NULL,
	[Data] NVARCHAR(max) NOT NULL,
	[CharacterId] INT NULL,
	[HistoricalEventId] INT NULL,
	[PlanetId] INT NULL,
	[SpeciesId] INT NULL,
	[UpdatedOn] DATETIME NOT NULL, 
	[AuthorId] INT NOT NULL, 
	CONSTRAINT [FK_DataRecords_Characters] FOREIGN KEY ([CharacterId]) REFERENCES [Characters]([Id]),
	CONSTRAINT [FK_DataRecords_HistoricalEvents] FOREIGN KEY ([HistoricalEventId]) REFERENCES [HistoricalEvents]([Id]),
	CONSTRAINT [FK_DataRecords_Planets] FOREIGN KEY ([PlanetId]) REFERENCES [Planets]([Id]),
	CONSTRAINT [FK_DataRecords_Species] FOREIGN KEY ([SpeciesId]) REFERENCES [Species]([Id]),
	CONSTRAINT [FK_DataRecords_Authors] FOREIGN KEY ([AuthorId]) REFERENCES [Authors]([Id]),
)

GO

CREATE INDEX [idx_DataRecords_CharacterId] ON [dbo].[DataRecords]([CharacterId]);
GO
CREATE INDEX [idx_DataRecords_HistoricalEventId] ON [dbo].[DataRecords]([HistoricalEventId]);
GO
CREATE INDEX [idx_DataRecords_PlanetId] ON [dbo].[DataRecords]([PlanetId]);
GO
CREATE INDEX [idx_DataRecords_SpeciesId] ON [dbo].[DataRecords]([SpeciesId]);
GO