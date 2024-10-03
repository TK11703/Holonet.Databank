CREATE PROCEDURE [dbo].[spPlanets_Insert]
	@Name nvarchar(150),
	@Description nvarchar(max),
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
		INSERT INTO dbo.Planets
			([Name], [Description], [Shard], [UpdatedOn], [AuthorId], [Active])
		Values	
			(@Name, @Description, @Shard, GETDATE(), @AuthorId, 1);

		SET @Id = SCOPE_IDENTITY();

		return 1;
	END
END		