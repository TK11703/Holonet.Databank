using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;

public record UpdatePlanetDto(
	[Required] int Id,
	[Required][StringLength(150)] string Name,
	string? Description,
	[Url][StringLength(500)] string? Shard,
	IEnumerable<string> Aliases
);