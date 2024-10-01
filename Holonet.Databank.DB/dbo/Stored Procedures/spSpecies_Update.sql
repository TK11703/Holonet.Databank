CREATE PROCEDURE [dbo].[spSpecies_Update]
	@Id int,
	@Name nvarchar(150),
	@Description nvarchar(max),
	@Shard nvarchar(500),
	@UpdatedBy nvarchar(250)
AS
BEGIN
	SET NOCOUNT OFF;

	UPDATE dbo.Species
		SET [Name]=@Name, [Description]=@Description, [Shard]=@Shard, [UpdatedOn]=GETDATE(), [UpdatedBy]=@UpdatedBy
	WHERE [Id]=@Id;

	IF (@@ROWCOUNT > 0)
	BEGIN
	   return 1;
	END
	ELSE
	BEGIN
	   return 0;
	END

END