CREATE FUNCTION [dbo].[funcAuthor_GetId]
(
	@AzureAuthorId uniqueidentifier
)
RETURNS INT
AS
BEGIN
	DECLARE @AuthorId int;
	SELECT TOP 1 @AuthorId = Id FROM dbo.Authors WHERE AzureId = @AzureAuthorId;

	IF @AuthorId IS NULL
	BEGIN
		SET @AuthorId = 0;
	END
	
	RETURN @AuthorId;	
END
