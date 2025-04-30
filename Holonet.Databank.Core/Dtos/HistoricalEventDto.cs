using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;

public record HistoricalEventDto(
	int Id,
	string Name,
	string? DatePeriod,
	IEnumerable<CharacterDto> Characters,
	IEnumerable<PlanetDto> Planets,
	IEnumerable<AliasDto> Aliases,
	IEnumerable<DataRecordDto> DataRecords,
	AuthorDto? UpdatedBy,
	DateTime? UpdatedOn
);