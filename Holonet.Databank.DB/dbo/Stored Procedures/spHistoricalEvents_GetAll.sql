CREATE PROCEDURE [dbo].[spHistoricalEvents_GetAll]

AS
BEGIN
	SELECT * 
	FROM dbo.HistoricalEvents
	WHERE [Active]=1
	ORDER BY [Name];
END