CREATE TABLE [dbo].[Characters]
(
	[Id] INT NOT NULL PRIMARY KEY identity,
	[PlanetId] INT NULL, 
	[FirstName] NVARCHAR(150) NOT NULL,
	[LastName] NVARCHAR(150) NOT NULL,
	[Description] NVARCHAR(max) NULL,
	[Shard] NVARCHAR(500) NULL,
	[BirthDate] NVARCHAR(200) NULL,
	[CreatedOn] DATETIME NOT NULL, 
	[CreatedBy] NVARCHAR(250) NOT NULL, 
	[UpdatedOn] DATETIME NOT NULL, 
	[UpdatedBy] NVARCHAR(250) NOT NULL, 
	[Active] BIT NOT NULL Default 1,
	CONSTRAINT [FK_Characters_Planets] FOREIGN KEY ([PlanetId]) REFERENCES [Planets]([Id]),
)
