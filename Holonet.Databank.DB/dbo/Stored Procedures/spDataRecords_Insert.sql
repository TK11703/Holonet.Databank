CREATE PROCEDURE [dbo].[spDataRecords_Insert]
	@Shard nvarchar(500) = null,
	@Data nvarchar(max) = null,
	@CharacterId int = null,
	@HistoricalEventId int = null,
	@PlanetId int = null,
	@SpeciesId int = null,
	@AzureAuthorId uniqueidentifier
AS
BEGIN

	DECLARE @AuthorId int;
	SET @AuthorId = dbo.funcAuthor_GetId(@AzureAuthorId);
	IF (@AuthorId = 0)
	BEGIN
		return 0;
	END
	ELSE
	BEGIN
		INSERT INTO dbo.DataRecords
			([Data], [Shard], [CharacterId], [HistoricalEventId], [PlanetId], [SpeciesId], [CreatedOn], [UpdatedOn], [CreatedAuthorId], [UpdatedAuthorId])
		Values
			(@Data, @Shard, @CharacterId, @HistoricalEventId, @PlanetId, @SpeciesId, GETDATE(), GETDATE(), @AuthorId, @AuthorId)

		return 1;		
	END

END		