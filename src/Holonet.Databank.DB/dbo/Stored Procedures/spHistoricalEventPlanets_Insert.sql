CREATE PROCEDURE [dbo].[spHistoricalEventPlanets_Insert]
	@TableData HistoricalEventPlanetUDT READONLY,
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
		INSERT INTO dbo.HistoricalEventPlanets
			([PlanetId], [HistoricalEventId], [UpdatedOn], [AuthorId])
		SELECT PlanetId, HistoricalEventId, GETDATE(), @AuthorId
			FROM @TableData;		

		return 1;	
	END

END		