﻿CREATE PROCEDURE [dbo].[spDataRecords_Insert]
	@Shard nvarchar(500) = null,
	@Data nvarchar(max) = null,
	@CharacterId int = null,
	@HistoricalEventId int = null,
	@PlanetId int = null,
	@SpeciesId int = null,
	@AzureAuthorId uniqueidentifier,
	@NewItemId int = null output
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
			([Data], [Shard], [IsNew], [IsProcessing], [IsProcessed], [CharacterId], [HistoricalEventId], [PlanetId], [SpeciesId], [CreatedOn], [UpdatedOn], [CreatedAuthorId], [UpdatedAuthorId])
		Values
			(@Data, @Shard, 1, 0, 0, @CharacterId, @HistoricalEventId, @PlanetId, @SpeciesId, GETDATE(), GETDATE(), @AuthorId, @AuthorId)
		SET @NewItemId = SCOPE_IDENTITY();
		return 1;		
	END

END		