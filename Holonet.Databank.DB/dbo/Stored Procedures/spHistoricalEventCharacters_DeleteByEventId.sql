CREATE PROCEDURE [dbo].[spHistoricalEventCharacters_DeleteByEventId]
	@EventId int = 0
AS
BEGIN

	SET NOCOUNT OFF;

	BEGIN TRY
		
		Delete from dbo.HistoricalEventCharacters
			where [HistoricalEventId] = @EventId;

		return 1;
	END TRY
	BEGIN CATCH
		return 0;
	END CATCH
END