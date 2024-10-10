CREATE PROCEDURE [dbo].[spHistoricalEventPlanets_GetByEventId]
	@HistoricalEventId int = 0
AS
	SELECT p.[Id], p.[Name] 
		FROM dbo.HistoricalEventPlanets as hep
		INNER JOIN dbo.Planets as p ON hep.PlanetId = p.Id
			WHERE hep.[HistoricalEventId] = @HistoricalEventId and p.Active=1;

	return 1;
RETURN 0
