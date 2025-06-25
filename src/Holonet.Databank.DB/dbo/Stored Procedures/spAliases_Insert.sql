CREATE PROCEDURE [dbo].[spAliases_Insert]
	@TableData AliasesUDT READONLY,
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
		INSERT INTO dbo.Aliases
			([Name], [CharacterId], [HistoricalEventId], [PlanetId], [SpeciesId], [UpdatedOn], [AuthorId])
		SELECT [AliasName], [CharacterId], [HistoricalEventId], [PlanetId], [SpeciesId], GETDATE(), @AuthorId
			FROM @TableData;

		return 1;		
	END

END		