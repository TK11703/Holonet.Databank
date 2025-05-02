using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Models;
using System.Numerics;

namespace Holonet.Databank.Core.Entities;

public static class EntityExtensions
{
	public static AliasDto ToDto(this Alias alias)
	{
		return new AliasDto
		(
			alias.Id,
			alias.Name,
			alias.CharacterId,
			alias.PlanetId,
			alias.SpeciesId,
			alias.HistoricalEventId,
			alias.UpdatedBy?.ToDto(),
			alias.UpdatedOn
		);
	}

	public static DataRecordDto ToDto(this DataRecord record)
	{
		return new DataRecordDto
		(
			record.Id,
            record.Shard ?? string.Empty,
            record.Data,			
            record.CharacterId,
			record.PlanetId,
			record.SpeciesId,
			record.HistoricalEventId,
            record.CreatedBy?.ToDto(),
            record.CreatedOn,
            record.UpdatedBy?.ToDto(),
			record.UpdatedOn
		);
	}

	public static AuthorDto ToDto(this Author author)
	{
		return new AuthorDto
		(
			author.Id,
			author.AzureId,
			author.DisplayName,
			author.Email ?? string.Empty
		);
	}

	public static PlanetDto ToDto(this Planet planet)
	{
		return new PlanetDto
		(
			planet.Id,
			planet.Name,
			planet.Aliases.Select(alias => alias.ToDto()),
			planet.DataRecords.Select(record => record.ToDto()),
			planet.UpdatedBy?.ToDto(),
			planet.UpdatedOn
		);
	}

	public static SpeciesDto ToDto(this Species species)
	{
		return new SpeciesDto
		(
			species.Id,
			species.Name,
			species.Aliases.Select(alias => alias.ToDto()),
			species.DataRecords.Select(record => record.ToDto()),
			species.UpdatedBy?.ToDto(),
			species.UpdatedOn
		);
	}

	public static HistoricalEventDto ToDto(this HistoricalEvent historicalEvent)
	{
		return new HistoricalEventDto
		(
			historicalEvent.Id,
			historicalEvent.Name,
			historicalEvent.DatePeriod,
			historicalEvent.Characters.Select(character => character.ToDto()),
			historicalEvent.Planets.Select(planet => planet.ToDto()),
			historicalEvent.Aliases.Select(alias => alias.ToDto()),
			historicalEvent.DataRecords.Select(record => record.ToDto()),
			historicalEvent.UpdatedBy?.ToDto(),
			historicalEvent.UpdatedOn
		);
	}

	public static CharacterDto ToDto(this Character character)
	{
		return new CharacterDto
		(
			character.Id,
			character.GivenName,
			character.FamilyName,
			character.BirthDate,
			character.Planet?.ToDto(),
			character.Species.Select(species => species.ToDto()),
			character.Aliases.Select(alias => alias.ToDto()),
			character.DataRecords.Select(record => record.ToDto()),
			character.UpdatedBy?.ToDto(),
			character.UpdatedOn

		);
	}

	public static PageResultDto<CharacterDto> ToDto(this PageResult<Character> pageResult)
	{
		return new PageResultDto<CharacterDto>
		(
			pageResult.Start,
			pageResult.PageSize,
			pageResult.ItemCount,
			pageResult.TotalPages,
			pageResult.CurrentPage,
			pageResult.IsFirstPage,
			pageResult.IsLastPage,
			pageResult.Collection.Select(character => character.ToDto())
		);
	}

	public static PageResultDto<PlanetDto> ToDto(this PageResult<Planet> pageResult)
	{
		return new PageResultDto<PlanetDto>
		(
			pageResult.Start,
			pageResult.PageSize,
			pageResult.ItemCount,
			pageResult.TotalPages,
			pageResult.CurrentPage,
			pageResult.IsFirstPage,
			pageResult.IsLastPage,
			pageResult.Collection.Select(planet => planet.ToDto())
		);
	}

	public static PageResultDto<SpeciesDto> ToDto(this PageResult<Species> pageResult)
	{
		return new PageResultDto<SpeciesDto>
		(
			pageResult.Start,
			pageResult.PageSize,
			pageResult.ItemCount,
			pageResult.TotalPages,
			pageResult.CurrentPage,
			pageResult.IsFirstPage,
			pageResult.IsLastPage,
			pageResult.Collection.Select(species => species.ToDto())
		);
	}

	public static PageResultDto<HistoricalEventDto> ToDto(this PageResult<HistoricalEvent> pageResult)
	{
		return new PageResultDto<HistoricalEventDto>
		(
			pageResult.Start,
			pageResult.PageSize,
			pageResult.ItemCount,
			pageResult.TotalPages,
			pageResult.CurrentPage,
			pageResult.IsFirstPage,
			pageResult.IsLastPage,
			pageResult.Collection.Select(historicalEvent => historicalEvent.ToDto())
		);
	}
}
