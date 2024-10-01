CREATE TABLE [dbo].[HistoricalEventPlanets]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[PlanetId] INT NOT NULL, 
	[HistoricalEventId] INT NOT NULL,
	[CreatedOn] DATETIME NOT NULL,
	[CreatedBy] NVARCHAR(250) NOT NULL,	
	CONSTRAINT [FK_HistoricalEventPlanets_Planets] FOREIGN KEY ([PlanetId]) REFERENCES [Planets]([Id]),
	CONSTRAINT [FK_HistoricalEventPlanets_HistoricalEvents] FOREIGN KEY ([HistoricalEventId]) REFERENCES [HistoricalEvents]([Id]),

)
