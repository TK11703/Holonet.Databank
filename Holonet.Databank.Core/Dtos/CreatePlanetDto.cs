using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;

public record CreatePlanetDto(
	[Required][StringLength(150)] string Name,
	string? Description,
	[Url][StringLength(500)] string? Shard
);
