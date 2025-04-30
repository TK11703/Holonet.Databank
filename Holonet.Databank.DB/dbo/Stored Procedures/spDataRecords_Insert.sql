CREATE PROCEDURE [dbo].[spDataRecords_Insert]
	@Shard nvarchar(500),
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
			([Data], [CharacterId], [HistoricalEventId], [PlanetId], [SpeciesId], [UpdatedOn], [AuthorId])
		Values
			(@Data, @CharacterId, @HistoricalEventId, @PlanetId, @SpeciesId, GETDATE(), @AuthorId)

		return 1;		
	END

END		