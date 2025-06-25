CREATE PROCEDURE [dbo].[spHistoricalEventCharacters_Insert]
	@TableData HistoricalEventCharacterUDT READONLY,
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
		INSERT INTO dbo.HistoricalEventCharacters
			([CharacterId], [HistoricalEventId], [UpdatedOn], [AuthorId])
		SELECT CharacterId, HistoricalEventId, GETDATE(), @AuthorId
			FROM @TableData;

		return 1;
	END
END		