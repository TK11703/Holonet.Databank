CREATE TABLE [dbo].[Characters]
(
	[Id] INT NOT NULL PRIMARY KEY identity,
	[PlanetId] INT NULL, 
	[GivenName] NVARCHAR(150) NOT NULL,
	[FamilyName] NVARCHAR(150) NULL,
	[BirthDate] NVARCHAR(200) NULL,
	[UpdatedOn] DATETIME NOT NULL, 
	[AuthorId] INT NOT NULL, 
	[Active] BIT NOT NULL Default 1,
	CONSTRAINT [FK_Characters_Planets] FOREIGN KEY ([PlanetId]) REFERENCES [Planets]([Id]),
	CONSTRAINT [FK_Characters_Authors] FOREIGN KEY ([AuthorId]) REFERENCES [Authors]([Id]),
)

GO
CREATE INDEX [idx_Characters_GivenName] ON [dbo].[Characters]([GivenName]);
GO
CREATE INDEX [idx_Characters_FamilyName] ON [dbo].[Characters]([FamilyName]);
GO
CREATE INDEX [idx_Characters_PlanetId] ON [dbo].[Characters]([PlanetId]);
GO
