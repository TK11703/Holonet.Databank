CREATE PROCEDURE [dbo].[spPlanets_GetAll]

AS
BEGIN
	SELECT * 
	FROM dbo.Planets
	WHERE [Active]=1
	ORDER BY [Name];
END