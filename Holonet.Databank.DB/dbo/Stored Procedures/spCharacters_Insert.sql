﻿CREATE PROCEDURE [dbo].[spCharacters_Insert]
	@PlanetId int,
	@GivenName nvarchar(150),
	@FamilyName nvarchar(150) = null,
	@BirthDate nvarchar(200),
	@AzureAuthorId uniqueidentifier,
	@Id int output
AS
BEGIN

	DECLARE @AuthorId int;
	SET @AuthorId = dbo.funcAuthor_GetId(@AzureAuthorId);
	IF (@AuthorId = 0)
	BEGIN
		SET @Id = 0;
		return 0;
	END
	ELSE
	BEGIN
		INSERT INTO dbo.Characters
			([PlanetId], [GivenName], [FamilyName], [BirthDate], [UpdatedOn], [AuthorId], [Active])
		Values	
			(@PlanetId, @GivenName, @FamilyName, @BirthDate, GETDATE(), @AuthorId, 1);

		SET @Id = SCOPE_IDENTITY();

		return 1;		
	END

END		