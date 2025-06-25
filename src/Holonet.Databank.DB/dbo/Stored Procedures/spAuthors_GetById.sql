CREATE PROCEDURE [dbo].[spAuthors_GetById]
	@Id int
AS
BEGIN
	SELECT top 1 * 
	FROM dbo.Authors
	WHERE [Id]=@Id;
END