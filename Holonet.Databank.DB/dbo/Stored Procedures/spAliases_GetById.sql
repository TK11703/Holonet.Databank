CREATE PROCEDURE [dbo].[spAliases_GetById]
	@CharacterId int = null,
	@PlanetId int = null,
	@SpeciesId int = null
AS
	SELECT * 
		FROM dbo.Aliases
			WHERE CharacterId=@CharacterId AND PlanetId=@PlanetId AND SpeciesId=@SpeciesId;		
	return 1;
RETURN 0
