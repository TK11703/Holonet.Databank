using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;
public record CharacterDto(
	int Id,
	string GivenName,
	string? FamilyName,
	string? BirthDate,
	PlanetDto? Planet,
    IEnumerable<SpeciesDto> Species,
	IEnumerable<AliasDto> Aliases,
	IEnumerable<DataRecordDto> DataRecords,
	AuthorDto? UpdatedBy,
	DateTime? UpdatedOn
);