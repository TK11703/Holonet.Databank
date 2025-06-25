CREATE PROCEDURE [dbo].[spHistoricalEvents_Get]
	@Id int
AS
BEGIN
	SELECT * 
	FROM dbo.HistoricalEvents
	WHERE [Id]=@Id AND [Active]=1;
END