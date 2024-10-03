CREATE TABLE [dbo].[Characters]
(
	[Id] INT NOT NULL PRIMARY KEY identity,
	[PlanetId] INT NULL, 
	[FirstName] NVARCHAR(150) NOT NULL,
	[LastName] NVARCHAR(150) NOT NULL,
	[Description] NVARCHAR(max) NULL,
	[Shard] NVARCHAR(500) NULL,
	[BirthDate] NVARCHAR(200) NULL,
	[UpdatedOn] DATETIME NOT NULL, 
	[AuthorId] INT NOT NULL, 
	[Active] BIT NOT NULL Default 1,
	CONSTRAINT [FK_Characters_Planets] FOREIGN KEY ([PlanetId]) REFERENCES [Planets]([Id]),
	CONSTRAINT [FK_Characters_Authors] FOREIGN KEY ([AuthorId]) REFERENCES [Authors]([Id]),
)
