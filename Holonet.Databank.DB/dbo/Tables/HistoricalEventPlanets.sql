CREATE TABLE [dbo].[HistoricalEventPlanets]
(
	[Id] INT NOT NULL PRIMARY KEY identity,
	[PlanetId] INT NOT NULL, 
	[HistoricalEventId] INT NOT NULL,
	[UpdatedOn] DATETIME NOT NULL, 
	[AuthorId] INT NOT NULL, 	
	CONSTRAINT [FK_HistoricalEventPlanets_Planets] FOREIGN KEY ([PlanetId]) REFERENCES [Planets]([Id]),
	CONSTRAINT [FK_HistoricalEventPlanets_HistoricalEvents] FOREIGN KEY ([HistoricalEventId]) REFERENCES [HistoricalEvents]([Id]),
	CONSTRAINT [FK_HistoricalEventPlanets_Authors] FOREIGN KEY ([AuthorId]) REFERENCES [Authors]([Id]),

)

GO
CREATE INDEX [idx_HistoricalEventPlanets_PlanetId] ON [dbo].[HistoricalEventPlanets]([PlanetId]);
GO
CREATE INDEX [idx_HistoricalEventPlanets_HistoricalEventId] ON [dbo].[HistoricalEventPlanets]([HistoricalEventId]);
GO
