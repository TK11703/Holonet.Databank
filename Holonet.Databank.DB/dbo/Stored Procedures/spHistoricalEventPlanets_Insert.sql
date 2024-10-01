CREATE PROCEDURE [dbo].[spHistoricalEventPlanets_Insert]
	@TableData HistoricalEventPlanetUDT READONLY,
	@CreatedBy nvarchar(250)
AS
BEGIN
	INSERT INTO dbo.HistoricalEventPlanets
		([PlanetId], [HistoricalEventId], [CreatedOn], [CreatedBy])
	SELECT PlanetId, HistoricalEventId, GETDATE(), @CreatedBy
		FROM @TableData;		

	return 1;
END		