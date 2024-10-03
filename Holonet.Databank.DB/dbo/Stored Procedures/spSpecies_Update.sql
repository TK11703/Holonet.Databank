CREATE PROCEDURE [dbo].[spSpecies_Update]
	@Id int,
	@Name nvarchar(150),
	@Description nvarchar(max),
	@Shard nvarchar(500),
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
		UPDATE dbo.Species
			SET [Name]=@Name, [Description]=@Description, [Shard]=@Shard, [UpdatedOn]=GETDATE(), [AuthorId]=@AuthorId
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