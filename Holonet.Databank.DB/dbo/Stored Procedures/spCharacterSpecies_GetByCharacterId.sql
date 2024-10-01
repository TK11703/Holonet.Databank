CREATE PROCEDURE [dbo].[spCharacterSpecies_GetByCharacterId]
	@CharacterId int = 0
AS
	SELECT * from dbo.CharacterSpecies
		where [CharacterId] = @CharacterId;
		
	return 1;
RETURN 0
