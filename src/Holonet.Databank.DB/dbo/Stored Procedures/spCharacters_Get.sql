CREATE PROCEDURE [dbo].[spCharacters_Get]
	@Id int
AS
BEGIN
	SELECT * 
	FROM dbo.Characters
	WHERE [Id]=@Id AND [Active]=1;
END