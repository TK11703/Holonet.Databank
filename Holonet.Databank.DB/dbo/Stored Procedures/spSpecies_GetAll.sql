CREATE PROCEDURE [dbo].[spSpecies_GetAll]

AS
BEGIN
	SELECT * 
	FROM dbo.Species
	WHERE [Active]=1
	ORDER BY [Name];
END