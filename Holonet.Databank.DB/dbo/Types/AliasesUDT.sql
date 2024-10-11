CREATE TYPE [dbo].[AliasesUDT] AS TABLE
(
	AliasName NVARCHAR(150) NOT NULL,
	CharacterId INT NULL,
	HistoricalEventId INT NULL,	
	PlanetId INT NULL,
	SpeciesId INT NULL
)