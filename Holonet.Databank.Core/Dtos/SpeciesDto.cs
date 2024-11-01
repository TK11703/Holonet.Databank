using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;

public record SpeciesDto(
	int Id,
	string Name,
	string? Shard,
	IEnumerable<AliasDto> Aliases,
	IEnumerable<DataRecordDto> DataRecords,
	AuthorDto? UpdatedBy,
	DateTime? UpdatedOn
);
