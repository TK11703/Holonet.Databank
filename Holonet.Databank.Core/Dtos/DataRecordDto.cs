using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;
public record DataRecordDto(
	int Id,
	string Data,
	int? CharacterId,
	int? PlanetId,
	int? SpeciesId,
	int? HistoricalEventId,
	AuthorDto? UpdatedBy,
	DateTime? UpdatedOn
);