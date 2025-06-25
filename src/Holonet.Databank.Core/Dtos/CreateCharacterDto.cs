using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;


public record CreateCharacterDto(
	[Required][StringLength(150)] string GivenName,
	[StringLength(150)] string? FamilyName,
	[StringLength(200)] string? BirthDate,
	int? PlanetId,
    IEnumerable<int> SpeciesIds,
	IEnumerable<string> Aliases,
	[Required] Guid AzureId
);