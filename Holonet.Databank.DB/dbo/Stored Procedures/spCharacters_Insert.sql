CREATE PROCEDURE [dbo].[spCharacters_Insert]
	@PlanetId int,
	@FirstName nvarchar(150),
	@LastName nvarchar(150),
	@Description nvarchar(max),
	@Shard nvarchar(500),
	@BirthDate nvarchar(200),
	@CreatedBy nvarchar(250),
	@Id int output
AS
BEGIN
	INSERT INTO dbo.Characters
		([PlanetId], [FirstName], [LastName], [Description], [Shard], [BirthDate], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdatedBy], [Active])
	Values	
		(@PlanetId, @FirstName, @LastName, @Description, @Shard, @BirthDate, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy, 1);

	SET @Id = SCOPE_IDENTITY();

	return 1;
END		