CREATE PROCEDURE [dbo].[spHistoricalEventPlanets_DeleteByEventId]
	@EventId int = 0
AS
BEGIN

	SET NOCOUNT OFF;

	BEGIN TRY
		
		Delete from dbo.HistoricalEventPlanets
			where [HistoricalEventId] = @EventId;

		return 1;
	END TRY
	BEGIN CATCH
		return 0;
	END CATCH
END