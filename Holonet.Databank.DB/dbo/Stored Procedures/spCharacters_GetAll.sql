CREATE PROCEDURE [dbo].[spCharacters_GetAll]

AS
BEGIN
	SELECT * 
	FROM dbo.Characters
	WHERE [Active]=1
	ORDER BY [FamilyName], [GivenName];
END