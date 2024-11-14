CREATE PROCEDURE [dbo].[spPlanets_GetAll]
	@UTCDate date = null
AS
BEGIN
	SELECT * 
	FROM dbo.Planets
	WHERE [Active]=1
	AND 
		((@UTCDate IS NULL) or [UpdatedOn] > @UTCDate)
	ORDER BY [Name];
END