CREATE PROCEDURE [dbo].[spHistoricalEventCharacters_GetByEventId]
	@HistoricalEventId int = 0
AS
	SELECT c.[Id], c.[GivenName], c.[FamilyName]
		FROM dbo.HistoricalEventCharacters as hec
		INNER JOIN dbo.Characters as c ON hec.CharacterId = c.Id
			WHERE hec.[HistoricalEventId] = @HistoricalEventId AND c.Active=1;

	return 1;
RETURN 0
