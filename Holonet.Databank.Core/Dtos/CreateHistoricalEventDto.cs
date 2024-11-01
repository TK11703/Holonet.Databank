﻿using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;

public record CreateHistoricalEventDto(
	[Required][StringLength(150)] string Name,
	[StringLength(200)] string? DatePeriod,
	[Url][StringLength(500)] string? Shard,
	IEnumerable<int> CharacterIds,
	IEnumerable<int> PlanetIds,
	IEnumerable<string> Aliases,
	[Required] Guid AzureId
);
