using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;

public record GetPlanetDto(
	[Required] int Id,
	[Required][StringLength(150)] string Name
);
