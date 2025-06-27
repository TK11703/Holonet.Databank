CREATE PROCEDURE [dbo].[spDataRecords_Exists]
	@Shard nvarchar(500),
	@CharacterId int = null,
	@HistoricalEventId int = null,
	@PlanetId int = null,
	@SpeciesId int = null
AS
BEGIN
	
	SELECT * 
	FROM dbo.DataRecords
	WHERE [Shard]=@Shard AND (
		(@CharacterId IS NOT NULL AND [CharacterId] = @CharacterId)
			OR (@HistoricalEventId IS NOT NULL AND [HistoricalEventId] = @HistoricalEventId)
			OR (@PlanetId IS NOT NULL AND [PlanetId] = @PlanetId)
			OR (@SpeciesId IS NOT NULL AND [SpeciesId] = @SpeciesId) 
		);

	IF @@ROWCOUNT > 0
	BEGIN
		RETURN 1;
	END
	ELSE
	BEGIN
		RETURN 0;
	END

END