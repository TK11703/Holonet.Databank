﻿using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;

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

	public static AliasModel ToAliasModel(this AliasDto alias)
	{
		return new AliasModel()
		{
			Id = alias.Id,
			Name = alias.Name
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
            character.SpeciesIds,
			character.Aliases.Select(alias => alias.Name)
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
            character.SpeciesIds,
			character.Aliases.Select(alias => alias.Name)
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
			Aliases = character.Aliases.Select(alias => alias.ToAliasModel()).ToList(),
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
			planet.Shard,
			planet.Aliases.Select(alias => alias.Name)
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
			planet.Aliases.Select(alias => alias.Name)
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
			Aliases = planet.Aliases.Select(alias => alias.ToAliasModel()).ToList(),
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
            species.Shard,
			species.Aliases.Select(alias => alias.Name)
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
			species.Aliases.Select(alias => alias.Name)
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
			Aliases = species.Aliases.Select(alias => alias.ToAliasModel()).ToList(),
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
			CharacterIds: historicalEvent.CharacterIds,
			Aliases: historicalEvent.Aliases.Select(alias => alias.Name)
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
			Aliases: historicalEvent.Aliases.Select(alias => alias.Name)
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
			Aliases = historicalEventDto.Aliases.Select(alias => alias.ToAliasModel()).ToList(),
			UpdatedBy = historicalEventDto.UpdatedBy?.ToAuthorModel(),
			UpdatedOn = historicalEventDto.UpdatedOn
		};
	}
}
