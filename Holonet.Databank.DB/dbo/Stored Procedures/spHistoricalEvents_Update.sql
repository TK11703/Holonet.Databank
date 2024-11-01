CREATE PROCEDURE [dbo].[spHistoricalEvents_Update]
	@Id int,
	@Name nvarchar(150),
	@DatePeriod nvarchar(200),
	@Shard nvarchar(500),
	@AzureAuthorId uniqueidentifier
AS
BEGIN
	SET NOCOUNT OFF;

	DECLARE @AuthorId int;
	SET @AuthorId = dbo.funcAuthor_GetId(@AzureAuthorId);
	IF (@AuthorId = 0)
	BEGIN
		return 0;
	END
	ELSE
	BEGIN
		UPDATE dbo.HistoricalEvents
			SET [Name]=@Name, [DatePeriod]=@DatePeriod, [Shard]=@Shard, [UpdatedOn]=GETDATE(), [AuthorId]=@AuthorId
		WHERE [Id]=@Id;

		IF (@@ROWCOUNT > 0)
		BEGIN
		   return 1;
		END
		ELSE
		BEGIN
		   return 0;
		END
	END

END