CREATE PROCEDURE [dbo].[spHistoricalEventCharacters_Insert]
	@TableData HistoricalEventCharacterUDT READONLY,
	@CreatedBy nvarchar(250)
AS
BEGIN
	INSERT INTO dbo.HistoricalEventCharacters
		([CharacterId], [HistoricalEventId], [CreatedOn], [CreatedBy])
	SELECT CharacterId, HistoricalEventId, GETDATE(), @CreatedBy
		FROM @TableData;

	return 1;
END		