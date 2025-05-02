CREATE PROCEDURE [dbo].[spDataRecords_Update]
	@Id int,
	@Shard nvarchar(500) = null,
	@Data nvarchar(max) = null,
	@CharacterId int = null,
	@HistoricalEventId int = null,
	@PlanetId int = null,
	@SpeciesId int = null,
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
		UPDATE dbo.DataRecords
			SET [Data]=@Data, [Shard]=@Shard, [UpdatedOn]=GETDATE(), [AuthorId]=@AuthorId,
				[CharacterId]=@CharacterId, [HistoricalEventId]=@HistoricalEventId, [PlanetId]=@PlanetId, [SpeciesId]=@SpeciesId
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