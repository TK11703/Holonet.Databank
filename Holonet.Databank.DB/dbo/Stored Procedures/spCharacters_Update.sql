CREATE PROCEDURE [dbo].[spCharacters_Update]
	@Id int,
	@PlanetId int,
	@GivenName nvarchar(150),
	@FamilyName nvarchar(150) = null,
	@Description nvarchar(max),
	@Shard nvarchar(500),
	@BirthDate nvarchar(200),
	@AzureAuthorId uniqueidentifier
AS
BEGIN
	SET NOCOUNT OFF;

	DECLARE @AuthorId int;
	SET @AuthorId = dbo.funcAuthor_GetId(@AzureAuthorId);
	IF (@AuthorId = 0)
	BEGIN
		return 0;
	END
	ELSE
	BEGIN
		UPDATE dbo.Characters
			SET [PlanetId]=@PlanetId, [GivenName]=@GivenName, [FamilyName]=@FamilyName, [Description]=@Description, [Shard]=@Shard, 
				[BirthDate]=@BirthDate, [UpdatedOn]=GETDATE(), [AuthorId]=@AuthorId
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

END
