CREATE PROCEDURE [dbo].[spSpecies_Insert]
	@Name nvarchar(150),
	@Shard nvarchar(500),
	@AzureAuthorId uniqueidentifier,
	@Id int output
AS
BEGIN

	DECLARE @AuthorId int;
	SET @AuthorId = dbo.funcAuthor_GetId(@AzureAuthorId);
	IF (@AuthorId = 0)
	BEGIN
		SET @Id = 0;
		return 0;
	END
	ELSE
	BEGIN
		INSERT INTO dbo.Species
			([Name], [Shard], [UpdatedOn], [AuthorId], [Active])
		Values	
			(@Name, @Shard, GETDATE(), @AuthorId, 1);

		SET @Id = SCOPE_IDENTITY();

		return 1;	
	END
	
END		