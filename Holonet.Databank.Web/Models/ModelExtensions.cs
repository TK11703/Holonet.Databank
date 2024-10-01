using Holonet.Databank.Core.Dtos;

namespace Holonet.Databank.Web.Models;

public static class ModelExtensions
{
	public static CreateCharacterDto ToCreateCharacterDto(this CharacterModel character)
	{
		return new CreateCharacterDto
		(
			character.FirstName,
			character.LastName,
			character.Description,
			character.Shard,
			character.BirthDate,
			character.PlanetId,
            character.SpeciesIds,
			character.CreatedBy
        );
	}

	public static UpdateCharacterDto ToUpdateCharacterDto(this CharacterModel character)
	{
		return new UpdateCharacterDto
		(
			character.Id,
			character.FirstName,
			character.LastName,
			character.Description,
			character.Shard,
			character.BirthDate,
			character.PlanetId,
            character.SpeciesIds,
			character.UpdatedBy
		);
	}

	public static CharacterModel ToCharacterModel(this CharacterDto character)
	{
		return new CharacterModel()
		{
			Id = character.Id,
			FirstName = character.FirstName,
			LastName = character.LastName,
			Description = character.Description,
			Shard = character.Shard,
			BirthDate = character.BirthDate,
			PlanetId = character.Planet?.Id,
			Planet = character.Planet?.ToPlanetModel(),
            SpeciesIds = character.Species.Select(s => s.Id),
            Species = character.Species.Select(s => s.ToSpeciesModel())
        };
	}

	public static CreatePlanetDto ToCreatePlanetDto(this PlanetModel planet)
	{
		return new CreatePlanetDto
		(
			planet.Name,
			planet.Description,
			planet.Shard,
			planet.CreatedBy
		);
	}

	public static UpdatePlanetDto ToUpdatePlanetDto(this PlanetModel planet)
	{
		return new UpdatePlanetDto
		(
			planet.Id,
			planet.Name,
			planet.Description,
			planet.Shard,
			planet.CreatedBy
		);
	}

	public static PlanetModel ToPlanetModel(this PlanetDto planet)
	{
		return new PlanetModel()
		{
			Id = planet.Id,
			Name = planet.Name,
			Description = planet.Description,
			Shard = planet.Shard
		};
	}

    public static CreateSpeciesDto ToCreateSpeciesDto(this SpeciesModel species)
    {
        return new CreateSpeciesDto
        (
            species.Name,
            species.Description,
            species.Shard,
			species.CreatedBy
		);
    }

    public static UpdateSpeciesDto ToUpdateSpeciesDto(this SpeciesModel species)
    {
        return new UpdateSpeciesDto
        (
            species.Id,
            species.Name,
            species.Description,
            species.Shard,
			species.UpdatedBy
		);
    }

    public static SpeciesModel ToSpeciesModel(this SpeciesDto species)
    {
        return new SpeciesModel()
        {
            Id = species.Id,
            Name = species.Name,
            Description = species.Description,
            Shard = species.Shard
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
			CharacterIds: historicalEvent.CharacterIds,
			CreatedBy: historicalEvent.CreatedBy
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
			CharacterIds: historicalEvent.CharacterIds,
			UpdatedBy: historicalEvent.UpdatedBy
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
			PlanetIds = historicalEventDto.Planets.Select(p => p.Id),
			Planets = historicalEventDto.Planets.Select(p=>p.ToPlanetModel()),
			CharacterIds = historicalEventDto.Characters.Select(c => c.Id),
			Characters = historicalEventDto.Characters.Select(c => c.ToCharacterModel())
		};
	}
}
