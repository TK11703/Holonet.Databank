CREATE PROCEDURE [dbo].[spCharacters_GetAll]
	@UTCDate date = null
AS
BEGIN
	SELECT * 
	FROM dbo.Characters
	WHERE [Active]=1 
	AND 
		((@UTCDate IS NULL) or [UpdatedOn] > @UTCDate)
	ORDER BY [FamilyName], [GivenName];
END