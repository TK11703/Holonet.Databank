using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;

public record CreateRecordDto(
	[Required] string Data,
	int? CharacterId,
	int? HistoricalEventId,
	int? PlanetId,
	int? SpeciesId,
	[Required] Guid AzureId
);
