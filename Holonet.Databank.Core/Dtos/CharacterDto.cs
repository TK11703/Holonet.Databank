using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;
public record CharacterDto(
	int Id,
	string FirstName,
	string LastName,
	string? Description,
	string? Shard,
	string? BirthDate,
	PlanetDto? Planet,
    IEnumerable<SpeciesDto> Species,
	AuthorDto? UpdatedBy,
	DateTime? UpdatedOn
);