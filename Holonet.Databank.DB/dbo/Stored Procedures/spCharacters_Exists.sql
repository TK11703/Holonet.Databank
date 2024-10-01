CREATE PROCEDURE [dbo].[spCharacters_Exists]
	@Id int,
	@PlanetId int,
	@FirstName nvarchar(150),
	@LastName nvarchar(150)
AS
BEGIN

	IF @Id = 0
	BEGIN
		SELECT * 
		FROM dbo.Characters
		WHERE [PlanetId]=@PlanetId AND [FirstName]=@FirstName AND [LastName]=@LastName AND [Active]=1;
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
		FROM dbo.Characters
		WHERE [Id]<>@Id AND [PlanetId]=@PlanetId AND [FirstName]=@FirstName AND [LastName]=@LastName AND [Active]=1;
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