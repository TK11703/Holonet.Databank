CREATE TABLE [dbo].[CharacterSpecies]
(
	[Id] INT NOT NULL PRIMARY KEY identity,
	[CharacterId] INT NOT NULL,
	[SpeciesId] INT NOT NULL,
	[UpdatedOn] DATETIME NOT NULL, 
	[AuthorId] INT NOT NULL, 
	CONSTRAINT [FK_SpeciesCharacters_Characters] FOREIGN KEY ([CharacterId]) REFERENCES [Characters]([Id]),
	CONSTRAINT [FK_SpeciesCharacters_Species] FOREIGN KEY ([SpeciesId]) REFERENCES [Species]([Id]),
	CONSTRAINT [FK_SpeciesCharacters_Authors] FOREIGN KEY ([AuthorId]) REFERENCES [Authors]([Id]),
)

GO
CREATE INDEX [idx_CharacterSpecies_CharacterId] ON [dbo].[CharacterSpecies]([CharacterId]);
GO
CREATE INDEX [idx_CharacterSpecies_SpeciesId] ON [dbo].[CharacterSpecies]([SpeciesId]);
GO
