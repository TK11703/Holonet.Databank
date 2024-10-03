CREATE PROCEDURE [dbo].[spAuthors_Update]
	@Id int,
	@AzureId UNIQUEIDENTIFIER,
	@DisplayName nvarchar(255),
	@Email nvarchar(255)
AS
BEGIN
	SET NOCOUNT OFF;

	IF EXISTS (SELECT 1 FROM dbo.Authors WHERE AzureId = @AzureId AND Id = @Id)
	BEGIN
		UPDATE dbo.Authors
			SET [DisplayName]=@DisplayName, [Email]=@Email
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
	ELSE
	BEGIN
		return 0;
	END

END