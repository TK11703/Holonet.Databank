﻿using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;
public record AliasDto(
	int Id,
	string Name,
	int? CharacterId,
	int? PlanetId,
	int? SpeciesId,
	AuthorDto? UpdatedBy,
	DateTime? UpdatedOn
);