CREATE PROCEDURE [dbo].[spCharacterSpecies_Insert]
	@TableData CharacterSpeciesUDT READONLY,
	@AzureAuthorId uniqueidentifier
AS
BEGIN

	DECLARE @AuthorId int;
	SET @AuthorId = dbo.funcAuthor_GetId(@AzureAuthorId);
	IF (@AuthorId = 0)
	BEGIN
		return 0;
	END
	ELSE
	BEGIN
		INSERT INTO dbo.CharacterSpecies
			([CharacterId], [SpeciesId], [UpdatedOn], [AuthorId])
		SELECT CharacterId, SpeciesId, GETDATE(), @AuthorId
			FROM @TableData;

		return 1;		
	END

END		