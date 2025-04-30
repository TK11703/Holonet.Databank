using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;

public record CreateRecordDto(
    [Url][StringLength(500)] string? Shard,
    [Required] string Data,
	int? CharacterId,
	int? HistoricalEventId,
	int? PlanetId,
	int? SpeciesId,
	[Required] Guid AzureId
);
