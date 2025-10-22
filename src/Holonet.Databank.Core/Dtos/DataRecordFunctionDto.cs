using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;
public record DataRecordFunctionDto(
	int Id,
    string? Shard,
    string? Data,
    int? CharacterId,
	int? PlanetId,
	int? SpeciesId,
	int? HistoricalEventId,
	DateTime? UpdatedOn
);