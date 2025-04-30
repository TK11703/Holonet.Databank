using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;

public record PlanetDto(
	int Id,
	string Name,
	IEnumerable<AliasDto> Aliases,
	IEnumerable<DataRecordDto> DataRecords,
	AuthorDto? UpdatedBy,
	DateTime? UpdatedOn
);
