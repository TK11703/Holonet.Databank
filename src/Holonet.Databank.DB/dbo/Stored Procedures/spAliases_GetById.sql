CREATE PROCEDURE [dbo].[spAliases_GetById]
	@CharacterId int = null,
	@HistoricalEventId int = null,
	@PlanetId int = null,
	@SpeciesId int = null
AS
	SELECT * 
		FROM [dbo].[Aliases]
			WHERE
				(@CharacterId IS NOT NULL AND [CharacterId] = @CharacterId)
				OR (@HistoricalEventId IS NOT NULL AND [HistoricalEventId] = @HistoricalEventId)
				OR (@PlanetId IS NOT NULL AND [PlanetId] = @PlanetId)
				OR (@SpeciesId IS NOT NULL AND [SpeciesId] = @SpeciesId);
	return 1;
RETURN 0
