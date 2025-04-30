using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;

public record UpdateHistoricalEventDto(
	[Required] int Id,
	[Required][StringLength(150)] string Name,
	[StringLength(200)] string? DatePeriod,
	IEnumerable<int> CharacterIds,
	IEnumerable<int> PlanetIds,
	IEnumerable<string> Aliases,
	[Required] Guid AzureId
);
