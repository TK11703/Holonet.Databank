using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Models;

namespace Holonet.Databank.Core.Entities;

public static class EntityExtensions
{
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
			planet.Description,
			planet.Shard,
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
            species.Description,
            species.Shard,
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
			historicalEvent.Description,
			historicalEvent.DatePeriod,
			historicalEvent.Shard,
			historicalEvent.Characters.Select(character => character.ToDto()),
			historicalEvent.Planets.Select(planet => planet.ToDto()),
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
			character.Description,
			character.Shard,
			character.BirthDate,
			character.Planet?.ToDto(),
            character.Species.Select(species => species.ToDto()),
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
