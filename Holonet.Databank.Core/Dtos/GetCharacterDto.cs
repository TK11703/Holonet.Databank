using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;

public record GetCharacterDto(
	[Required] int Id,
	[Required][StringLength(150)] string FirstName,
	[Required][StringLength(150)] string LastName,
	int? PlanetId
);
