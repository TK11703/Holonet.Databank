CREATE PROCEDURE [dbo].[spCharacters_Exists]
	@Id int,
	@PlanetId int,
	@GivenName nvarchar(150),
	@FamilyName nvarchar(150) = null
AS
BEGIN

	IF @Id = 0
	BEGIN
		SELECT * 
		FROM dbo.Characters
		WHERE [PlanetId]=@PlanetId AND [GivenName]=@GivenName AND [FamilyName]=@FamilyName AND [Active]=1;
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
		WHERE [Id]<>@Id AND [PlanetId]=@PlanetId AND [GivenName]=@GivenName AND [FamilyName]=@FamilyName AND [Active]=1;
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