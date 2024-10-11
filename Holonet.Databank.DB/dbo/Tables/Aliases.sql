CREATE TABLE [dbo].[Aliases]
(
	[Id] INT NOT NULL PRIMARY KEY identity,
	[Name] NVARCHAR(150) NOT NULL,
	[CharacterId] INT NULL,
	[PlanetId] INT NULL,
	[SpeciesId] INT NULL,
	[UpdatedOn] DATETIME NOT NULL, 
	[AuthorId] INT NOT NULL, 
	CONSTRAINT [FK_Aliases_Characters] FOREIGN KEY ([CharacterId]) REFERENCES [Characters]([Id]),
	CONSTRAINT [FK_Aliases_Planets] FOREIGN KEY ([PlanetId]) REFERENCES [Planets]([Id]),
	CONSTRAINT [FK_Aliases_Species] FOREIGN KEY ([SpeciesId]) REFERENCES [Species]([Id]),
	CONSTRAINT [FK_Aliases_Authors] FOREIGN KEY ([AuthorId]) REFERENCES [Authors]([Id]),
)

GO

CREATE INDEX [idx_Aliases_Name] ON [dbo].[Aliases]([Name]);
GO
CREATE INDEX [idx_Aliases_CharacterId] ON [dbo].[Aliases]([CharacterId]);
GO
CREATE INDEX [idx_Aliases_PlanetId] ON [dbo].[Aliases]([PlanetId]);
GO
CREATE INDEX [idx_Aliases_SpeciesId] ON [dbo].[Aliases]([SpeciesId]);
GO