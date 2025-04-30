CREATE PROCEDURE [dbo].[spHistoricalEvents_Insert]
	@Name nvarchar(150),
	@DatePeriod nvarchar(200),
	@AzureAuthorId uniqueidentifier,
	@Id int output
AS
BEGIN
	
	DECLARE @AuthorId int;
	SET @AuthorId = dbo.funcAuthor_GetId(@AzureAuthorId);
	IF (@AuthorId = 0)
	BEGIN
		SET @Id = 0;
		return 0;
	END
	ELSE
	BEGIN
		INSERT INTO dbo.HistoricalEvents
			([Name], [DatePeriod], [UpdatedOn], [AuthorId], [Active])
		Values
			(@Name, @DatePeriod, GETDATE(), @AuthorId, 1);

		SET @Id = SCOPE_IDENTITY();

		return 1;
	END
END		