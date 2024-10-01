CREATE PROCEDURE [dbo].[spCharacters_Update]
	@Id int,
	@PlanetId int,
	@FirstName nvarchar(150),
	@LastName nvarchar(150),
	@Description nvarchar(max),
	@Shard nvarchar(500),
	@BirthDate nvarchar(200),
	@UpdatedBy nvarchar(250)
AS
BEGIN
	SET NOCOUNT OFF;

	UPDATE dbo.Characters
		SET [PlanetId]=@PlanetId, [FirstName]=@FirstName, [LastName]=@LastName, [Description]=@Description, [Shard]=@Shard, 
			[BirthDate]=@BirthDate, [UpdatedOn]=GETDATE(), [UpdatedBy]=@UpdatedBy
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
