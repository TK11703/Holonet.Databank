CREATE PROCEDURE [dbo].[spAliases_GetById]
	@CharacterId int = null,
	@HistoricalEventId int = null,
	@PlanetId int = null,
	@SpeciesId int = null
AS
	SELECT * 
		FROM dbo.Aliases
			WHERE CharacterId=@CharacterId AND HistoricalEventId=@HistoricalEventId AND PlanetId=@PlanetId AND SpeciesId=@SpeciesId;		
	return 1;
RETURN 0
