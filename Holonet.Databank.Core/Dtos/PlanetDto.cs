﻿using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;

public record PlanetDto(
	int Id,
	string Name,
	string? Description,
	string? Shard,
	IEnumerable<AliasDto> Aliases,
	AuthorDto? UpdatedBy,
	DateTime? UpdatedOn
);
