CREATE PROCEDURE [dbo].[spHistoricalEventCharacters_GetByEventId]
	@EventId int = 0
AS
	SELECT * from dbo.HistoricalEventCharacters
			where [HistoricalEventId] = @EventId;

	return 1;
RETURN 0
