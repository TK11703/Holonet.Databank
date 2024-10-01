﻿using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;

public record UpdateCharacterDto(
	[Required] int Id,
	[Required][StringLength(150)] string FirstName,
	[Required][StringLength(150)] string LastName,
	string? Description,
	[Url][StringLength(500)] string? Shard,
	[StringLength(200)] string? BirthDate,
	int? PlanetId,
    IEnumerable<int> SpeciesIds,
	[Required][StringLength(250)] string UpdatedBy
);
