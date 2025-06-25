CREATE PROCEDURE [dbo].[spPlanets_Get]
	@Id int
AS
BEGIN
	SELECT * 
	FROM dbo.Planets
	WHERE [Id]=@Id AND [Active]=1;
END