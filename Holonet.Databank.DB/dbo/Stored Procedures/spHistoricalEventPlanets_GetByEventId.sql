CREATE PROCEDURE [dbo].[spHistoricalEventPlanets_GetByEventId]
	@EventId int = 0
AS
	SELECT * from dbo.HistoricalEventPlanets
			where [HistoricalEventId] = @EventId;

	return 1;
RETURN 0
