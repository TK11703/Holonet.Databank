CREATE PROCEDURE [dbo].[spSpecies_Get]
	@Id int
AS
BEGIN
	SELECT * 
	FROM dbo.Species
	WHERE [Id]=@Id AND [Active]=1;
END