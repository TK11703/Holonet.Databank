using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;

public record GetCharacterDto(
	[Required] int Id,
	[Required][StringLength(150)] string GivenName,
	[StringLength(150)] string? FamilyName,
	int? PlanetId
);
