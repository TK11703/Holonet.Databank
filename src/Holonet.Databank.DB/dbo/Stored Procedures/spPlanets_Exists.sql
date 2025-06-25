CREATE PROCEDURE [dbo].[spPlanets_Exists]
	@Id int,
	@Name nvarchar(150)
AS
BEGIN
	IF @Id = 0
	BEGIN
		SELECT * 
		FROM dbo.Planets
		WHERE [Name]=@Name AND [Active]=1;
		IF @@ROWCOUNT > 0
		BEGIN
			RETURN 1;
		END
		ELSE
		BEGIN
			RETURN 0;
		END
	END
	ELSE
	BEGIN
		SELECT * 
		FROM dbo.Planets
		WHERE [Id]<>@Id AND [Name]=@Name AND [Active]=1;
		IF @@ROWCOUNT > 0
		BEGIN
			RETURN 1;
		END
		ELSE
		BEGIN
			RETURN 0;
		END
	END	
END