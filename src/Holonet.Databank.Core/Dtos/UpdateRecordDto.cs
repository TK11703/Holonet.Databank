using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;

public record UpdateRecordDto(
	[Required] int Id,
	[Url][StringLength(500)] string? Shard,
    [Required] string Data,
    bool IsNew,
    bool IsProcessing,
    bool IsProcessed,
    string? SystemMessage,
    int? CharacterId,
    int? HistoricalEventId,
    int? PlanetId,
    int? SpeciesId,
    [Required] Guid UpdatedAzureId
);