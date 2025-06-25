CREATE PROCEDURE [dbo].[spSpecies_Delete]
	@Id int = 0
AS
BEGIN

	SET NOCOUNT OFF;

	BEGIN TRY
		--Shouldn't do a complete delete, because it will leave remnants
		-- Delete from dbo.Species	where [Id] = @Id
		Update dbo.Species
		Set [Active] = 0
		WHERE [Id]=@Id;

		return 1;
	END TRY
	BEGIN CATCH
		return 0;
	END CATCH
END