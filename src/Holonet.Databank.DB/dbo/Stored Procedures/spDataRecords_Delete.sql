CREATE PROCEDURE [dbo].[spDataRecords_Delete]
	@Id int = 0,
	@CharacterId int = null,
	@HistoricalEventId int = null,
	@PlanetId int = null,
	@SpeciesId int = null
AS
BEGIN

	SET NOCOUNT OFF;

	BEGIN TRY
		
		DELETE FROM dbo.DataRecords		
		WHERE [Id]=@Id AND (
			(@CharacterId IS NOT NULL AND [CharacterId] = @CharacterId)
				OR (@HistoricalEventId IS NOT NULL AND [HistoricalEventId] = @HistoricalEventId)
				OR (@PlanetId IS NOT NULL AND [PlanetId] = @PlanetId)
				OR (@SpeciesId IS NOT NULL AND [SpeciesId] = @SpeciesId) 
			);

		return 1;
	END TRY
	BEGIN CATCH
		return 0;
	END CATCH
END