CREATE PROCEDURE [dbo].[spAliases_DeleteById]
	@CharacterId int = null,
	@HistoricalEventId int = null,
	@PlanetId int = null,
	@SpeciesId int = null
AS
BEGIN

	SET NOCOUNT OFF;

	BEGIN TRY
		
		DELETE 
			FROM dbo.Aliases
				WHERE [CharacterId] = @CharacterId AND [HistoricalEventId] = @HistoricalEventId AND [PlanetId] = @PlanetId AND [SpeciesId] = @SpeciesId;

		return 1;
	END TRY
	BEGIN CATCH
		return 0;
	END CATCH
END