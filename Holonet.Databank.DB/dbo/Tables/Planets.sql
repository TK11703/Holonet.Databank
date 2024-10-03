CREATE TABLE [dbo].[Planets]
(
	[Id] INT NOT NULL PRIMARY KEY identity,
	[Name] NVARCHAR(150) NOT NULL,
	[Description] NVARCHAR(MAX) NULL,
	[Shard] NVARCHAR(500) NULL,
	[UpdatedOn] DATETIME NOT NULL, 
	[AuthorId] INT NOT NULL, 
	[Active] BIT NOT NULL Default 1,
	CONSTRAINT [FK_Planets_Authors] FOREIGN KEY ([AuthorId]) REFERENCES [Authors]([Id]),
)
