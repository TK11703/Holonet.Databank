﻿using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;

public record CreatePlanetDto(
	[Required][StringLength(150)] string Name,
	[Url][StringLength(500)] string? Shard,
	IEnumerable<string> Aliases,
	[Required] Guid AzureId
);
