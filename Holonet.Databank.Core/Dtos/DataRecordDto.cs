using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;
public record DataRecordDto(
	int Id,
    string? Shard,
    string? Data,
	int? CharacterId,
	int? PlanetId,
	int? SpeciesId,
	int? HistoricalEventId,
    AuthorDto? CreatedBy,
    DateTime? CreatedOn,
    AuthorDto? UpdatedBy,
	DateTime? UpdatedOn
);