CREATE TABLE [dbo].[CharacterSpecies]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[CharacterId] INT NOT NULL,
	[SpeciesId] INT NOT NULL,
	[CreatedOn] DATETIME NOT NULL,
	[CreatedBy] NVARCHAR(250) NOT NULL,
	CONSTRAINT [FK_SpeciesCharacters_Characters] FOREIGN KEY ([CharacterId]) REFERENCES [Characters]([Id]),
	CONSTRAINT [FK_SpeciesCharacters_Species] FOREIGN KEY ([SpeciesId]) REFERENCES [Species]([Id]),
)
