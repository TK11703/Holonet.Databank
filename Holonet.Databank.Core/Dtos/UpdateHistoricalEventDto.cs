using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;

public record UpdateHistoricalEventDto(
	[Required] int Id,
	[Required][StringLength(150)] string Name,
	string? Description,
	[StringLength(200)] string? DatePeriod,
	[Url][StringLength(500)] string? Shard,
	IEnumerable<int> CharacterIds,
	IEnumerable<int> PlanetIds,
	[Required][StringLength(250)] string UpdatedBy
);
