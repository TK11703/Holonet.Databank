CREATE TABLE [dbo].[Planets]
(
	[Id] INT NOT NULL PRIMARY KEY identity,
	[Name] NVARCHAR(150) NOT NULL,
	[UpdatedOn] DATETIME NOT NULL, 
	[AuthorId] INT NOT NULL, 
	[Active] BIT NOT NULL Default 1,
	CONSTRAINT [FK_Planets_Authors] FOREIGN KEY ([AuthorId]) REFERENCES [Authors]([Id]),
)

GO
CREATE INDEX [idx_Planets_Name] ON [dbo].[Planets]([Name]);
GO
