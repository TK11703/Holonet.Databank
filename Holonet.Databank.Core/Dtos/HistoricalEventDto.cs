using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;

public record HistoricalEventDto(
	int Id,
	string Name,
	string? Description,
	string? DatePeriod,
	string? Shard,
	IEnumerable<CharacterDto> Characters,
	IEnumerable<PlanetDto> Planets,
	IEnumerable<AliasDto> Aliases,
	AuthorDto? UpdatedBy,
	DateTime? UpdatedOn
);