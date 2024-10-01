﻿using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;

public record UpdateSpeciesDto(
	[Required] int Id,
	[Required][StringLength(150)] string Name,
	string? Description,
	[Url][StringLength(500)] string? Shard,
	[Required][StringLength(250)] string UpdatedBy
);