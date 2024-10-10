using Holonet.Databank.Core.Dtos;

namespace Holonet.Databank.Web.Models;

public static class ModelExtensions
{
	public static CreateAuthorDto ToCreateAuthorDto(this AuthorModel author)
	{
		return new CreateAuthorDto
		(
			author.AzureId,
			author.DisplayName,
			author.Email ?? string.Empty
		);
	}

	public static UpdateAuthorDto ToUpdateAuthorDto(this AuthorModel author)
	{
		return new UpdateAuthorDto
		(
			author.Id,
			author.AzureId,
			author.DisplayName,
			author.Email ?? string.Empty
		);
	}

	public static AuthorModel ToAuthorModel(this AuthorDto author)
	{
		return new AuthorModel()
		{
			Id = author.Id,
			AzureId = author.AzureId,
			DisplayName = author.DisplayName,
			Email = author.Email
		};
	}

	public static CreateCharacterDto ToCreateCharacterDto(this CharacterModel character)
	{
		return new CreateCharacterDto
		(
			character.GivenName,
			character.FamilyName,
			character.Description,
			character.Shard,
			character.BirthDate,
			character.PlanetId,
            character.SpeciesIds
        );
	}

	public static UpdateCharacterDto ToUpdateCharacterDto(this CharacterModel character)
	{
		return new UpdateCharacterDto
		(
			character.Id,
			character.GivenName,
			character.FamilyName,
			character.Description,
			character.Shard,
			character.BirthDate,
			character.PlanetId,
            character.SpeciesIds
		);
	}

	public static CharacterModel ToCharacterModel(this CharacterDto character)
	{
		return new CharacterModel()
		{
			Id = character.Id,
			GivenName = character.GivenName,
			FamilyName = character.FamilyName,
			Description = character.Description,
			Shard = character.Shard,
			BirthDate = character.BirthDate,
			PlanetId = character.Planet?.Id,
			Planet = character.Planet?.ToPlanetModel(),
            SpeciesIds = character.Species.Select(s => s.Id).ToList(),
            Species = character.Species.Select(s => s.ToSpeciesModel()),
			UpdatedBy = character.UpdatedBy?.ToAuthorModel(),
			UpdatedOn = character.UpdatedOn
		};
	}

	public static CreatePlanetDto ToCreatePlanetDto(this PlanetModel planet)
	{
		return new CreatePlanetDto
		(
			planet.Name,
			planet.Description,
			planet.Shard
		);
	}

	public static UpdatePlanetDto ToUpdatePlanetDto(this PlanetModel planet)
	{
		return new UpdatePlanetDto
		(
			planet.Id,
			planet.Name,
			planet.Description,
			planet.Shard
		);
	}

	public static PlanetModel ToPlanetModel(this PlanetDto planet)
	{
		return new PlanetModel()
		{
			Id = planet.Id,
			Name = planet.Name,
			Description = planet.Description,
			Shard = planet.Shard,
			UpdatedBy = planet.UpdatedBy?.ToAuthorModel(),
			UpdatedOn = planet.UpdatedOn
		};
	}

    public static CreateSpeciesDto ToCreateSpeciesDto(this SpeciesModel species)
    {
        return new CreateSpeciesDto
        (
            species.Name,
            species.Description,
            species.Shard
		);
    }

    public static UpdateSpeciesDto ToUpdateSpeciesDto(this SpeciesModel species)
    {
        return new UpdateSpeciesDto
        (
            species.Id,
            species.Name,
            species.Description,
            species.Shard
		);
    }

    public static SpeciesModel ToSpeciesModel(this SpeciesDto species)
    {
        return new SpeciesModel()
        {
            Id = species.Id,
            Name = species.Name,
            Description = species.Description,
            Shard = species.Shard,
			UpdatedBy = species.UpdatedBy?.ToAuthorModel(),
			UpdatedOn = species.UpdatedOn
		};
    }

    public static CreateHistoricalEventDto ToCreateHistoricalEventDto(this HistoricalEventModel historicalEvent)
	{
		return new CreateHistoricalEventDto
		(
			Name: historicalEvent.Name,
			Description: historicalEvent.Description,
			Shard: historicalEvent.Shard,
			DatePeriod: historicalEvent.DatePeriod,
			PlanetIds: historicalEvent.PlanetIds,
			CharacterIds: historicalEvent.CharacterIds
		);
	}

	public static UpdateHistoricalEventDto ToUpdateHistoricalEventDto(this HistoricalEventModel historicalEvent)
	{
		return new UpdateHistoricalEventDto
		(
			Id: historicalEvent.Id,
			Name: historicalEvent.Name,
			Description: historicalEvent.Description,
			Shard: historicalEvent.Shard,
			DatePeriod: historicalEvent.DatePeriod,
			PlanetIds: historicalEvent.PlanetIds,
			CharacterIds: historicalEvent.CharacterIds
		);
	}

	public static HistoricalEventModel ToHistoricalEventModel(this HistoricalEventDto historicalEventDto)
	{
		return new HistoricalEventModel()
		{
			Id = historicalEventDto.Id,
			Name = historicalEventDto.Name,
			Description = historicalEventDto.Description,
			Shard = historicalEventDto.Shard,
			DatePeriod = historicalEventDto.DatePeriod,
			PlanetIds = historicalEventDto.Planets.Select(p => p.Id).ToList(),
			Planets = historicalEventDto.Planets.Select(p=>p.ToPlanetModel()),
			CharacterIds = historicalEventDto.Characters.Select(c => c.Id).ToList(),
			Characters = historicalEventDto.Characters.Select(c => c.ToCharacterModel()),
			UpdatedBy = historicalEventDto.UpdatedBy?.ToAuthorModel(),
			UpdatedOn = historicalEventDto.UpdatedOn
		};
	}
}
