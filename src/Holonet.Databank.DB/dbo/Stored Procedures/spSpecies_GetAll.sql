CREATE PROCEDURE [dbo].[spSpecies_GetAll]
	@UTCDate date = null
AS
BEGIN
	SELECT * 
	FROM dbo.Species
	WHERE [Active]=1
	AND 
		((@UTCDate IS NULL) or [UpdatedOn] > @UTCDate)
	ORDER BY [Name];
END