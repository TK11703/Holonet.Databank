CREATE PROCEDURE [dbo].[spCharacterSpecies_DeleteByCharacterId]
	@CharacterId int = 0
AS
BEGIN

	SET NOCOUNT OFF;

	BEGIN TRY
		
		Delete from dbo.CharacterSpecies
			where [CharacterId] = @CharacterId;

		return 1;
	END TRY
	BEGIN CATCH
		return 0;
	END CATCH
END