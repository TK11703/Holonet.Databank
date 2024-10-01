CREATE PROCEDURE [dbo].[spPlanets_Insert]
	@Name nvarchar(150),
	@Description nvarchar(max),
	@Shard nvarchar(500),
	@CreatedBy nvarchar(250),
	@Id int output
AS
BEGIN
	INSERT INTO dbo.Planets
		([Name], [Description], [Shard], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdatedBy], [Active])
	Values	
		(@Name, @Description, @Shard, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy, 1);

	SET @Id = SCOPE_IDENTITY();

	return 1;
END		