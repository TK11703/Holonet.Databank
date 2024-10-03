CREATE PROCEDURE [dbo].[spCharacters_Insert]
	@PlanetId int,
	@FirstName nvarchar(150),
	@LastName nvarchar(150),
	@Description nvarchar(max),
	@Shard nvarchar(500),
	@BirthDate nvarchar(200),
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
		INSERT INTO dbo.Characters
			([PlanetId], [FirstName], [LastName], [Description], [Shard], [BirthDate], [UpdatedOn], [AuthorId], [Active])
		Values	
			(@PlanetId, @FirstName, @LastName, @Description, @Shard, @BirthDate, GETDATE(), @AuthorId, 1);

		SET @Id = SCOPE_IDENTITY();

		return 1;		
	END

END		