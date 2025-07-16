CREATE PROCEDURE [dbo].[spDataRecords_Update]
	@Id int,
	@Shard nvarchar(500) = null,
	@Data nvarchar(max) = null,
	@IsProcessing bit = 0,
	@IsProcessed bit = 0,
	@SystemMessage nvarchar(150) = null,
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
		DECLARE @IsNew bit;
		SELECT @IsNew = [IsNew] FROM dbo.DataRecords WHERE [Id]=@Id;
		IF(@IsProcessed = 1 OR @IsProcessing = 1 OR @SystemMessage IS NOT NULL)
		BEGIN
			SET @IsNew = 0;
		END
		
		UPDATE dbo.DataRecords
			SET [Data]=@Data, [Shard]=@Shard, 
				[IsNew]=@IsNew, [IsProcessing]=@IsProcessing, [IsProcessed]=@IsProcessed, [SystemMessage]=@SystemMessage, [UpdatedOn]=GETDATE(), [UpdatedAuthorId]=@AuthorId,
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