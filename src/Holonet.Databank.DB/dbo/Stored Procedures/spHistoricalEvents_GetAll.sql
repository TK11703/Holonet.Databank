CREATE PROCEDURE [dbo].[spHistoricalEvents_GetAll]
	@UTCDate date = null
AS
BEGIN
	SELECT * 
	FROM dbo.HistoricalEvents
	WHERE [Active]=1
	AND 
		((@UTCDate IS NULL) or [UpdatedOn] > @UTCDate)
	ORDER BY [Name];
END