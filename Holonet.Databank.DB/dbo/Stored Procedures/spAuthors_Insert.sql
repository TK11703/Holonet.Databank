CREATE PROCEDURE [dbo].[spAuthors_Insert]
	@AzureId UNIQUEIDENTIFIER,
	@DisplayName nvarchar(255),
	@Email nvarchar(255),
	@Id int output
AS
BEGIN

	DECLARE @AuthorId int;
	SET @AuthorId = dbo.funcAuthor_GetId(@AzureId);
	IF (@AuthorId = 0)
	BEGIN
		INSERT INTO dbo.Authors
			([AzureId], [DisplayName], [Email])
		Values	
			(@AzureId, @DisplayName, @Email);

		SET @Id = SCOPE_IDENTITY();

		return 1;		
	END
	ELSE
	BEGIN
		SET @Id = 0;
		return 0;
	END
	
END		