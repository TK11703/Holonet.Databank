CREATE PROCEDURE [dbo].[spAuthors_GetByAzureId]
	@AzureId UNIQUEIDENTIFIER
AS
BEGIN
	SELECT top 1 * 
	FROM dbo.Authors
	WHERE [AzureId]=@AzureId;
END