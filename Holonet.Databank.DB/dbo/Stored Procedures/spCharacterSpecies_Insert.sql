CREATE PROCEDURE [dbo].[spCharacterSpecies_Insert]
	@TableData CharacterSpeciesUDT READONLY,
	@CreatedBy nvarchar(250)
AS
BEGIN
	INSERT INTO dbo.CharacterSpecies
		([CharacterId], [SpeciesId], [CreatedOn], [CreatedBy])
	SELECT CharacterId, SpeciesId, GETDATE(), @CreatedBy
		FROM @TableData;

	return 1;
END		