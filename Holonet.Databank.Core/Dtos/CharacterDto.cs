using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;
public record CharacterDto(
	int Id,
	string GivenName,
	string? FamilyName,
	string? Description,
	string? Shard,
	string? BirthDate,
	PlanetDto? Planet,
    IEnumerable<SpeciesDto> Species,
	IEnumerable<AliasDto> Aliases,
	AuthorDto? UpdatedBy,
	DateTime? UpdatedOn
);