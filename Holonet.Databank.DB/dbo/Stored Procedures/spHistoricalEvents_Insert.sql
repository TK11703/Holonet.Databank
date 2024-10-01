CREATE PROCEDURE [dbo].[spHistoricalEvents_Insert]
	@Name nvarchar(150),
	@Description nvarchar(max),
	@DatePeriod nvarchar(200),
	@Shard nvarchar(500),
	@CreatedBy nvarchar(250),
	@Id int output
AS
BEGIN
	INSERT INTO dbo.HistoricalEvents
		([Name], [Description], [DatePeriod], [Shard], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdatedBy], [Active])
	Values
		(@Name, @Description, @DatePeriod, @Shard, GETDATE(), @CreatedBy, GETDATE(), @CreatedBy, 1);

	SET @Id = SCOPE_IDENTITY();

	return 1;
END		