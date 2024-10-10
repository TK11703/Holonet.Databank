using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;

public record UpdateCharacterDto(
	[Required] int Id,
	[Required][StringLength(150)] string GivenName,
	[StringLength(150)] string? FamilyName,
	string? Description,
	[Url][StringLength(500)] string? Shard,
	[StringLength(200)] string? BirthDate,
	int? PlanetId,
    IEnumerable<int> SpeciesIds
);
