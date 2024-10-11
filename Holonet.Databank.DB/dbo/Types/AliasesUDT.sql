CREATE TYPE [dbo].[AliasesUDT] AS TABLE
(
	AliasName NVARCHAR(150) NOT NULL,
	CharacterId INT NULL,
	SpeciesId INT NULL,
	PlanetId INT NULL
)