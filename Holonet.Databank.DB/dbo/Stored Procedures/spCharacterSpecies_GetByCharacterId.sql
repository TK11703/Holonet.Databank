CREATE PROCEDURE [dbo].[spCharacterSpecies_GetByCharacterId]
	@CharacterId int = 0
AS
	SELECT s.[Id], s.[Name] 
		FROM dbo.CharacterSpecies as cs
		INNER JOIN dbo.Species as s ON cs.SpeciesId = s.Id
			WHERE cs.[CharacterId] = @CharacterId AND s.Active=1;
		
	return 1;
RETURN 0
