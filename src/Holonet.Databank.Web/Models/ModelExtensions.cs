using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;

namespace Holonet.Databank.Web.Models;

public static class ModelExtensions
{
    public static ChatRequestDto ToChatRequestDto(this ChatRequestModel model)
    {
        return new ChatRequestDto(model.Prompt, model.AzureId);
    }
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

    public static CreateRecordDto ToCreateRecordDto(this DataRecordModel record)
    {
        return new CreateRecordDto
        (
            record.Shard ?? string.Empty,
            record.Data ?? string.Empty,
            record.CharacterId,
            record.HistoricalEventId,
            record.PlanetId,
            record.SpeciesId,
            CreatedAzureId: record.CreatedBy?.AzureId ?? Guid.Empty
        );
    }

    public static DataRecordFunctionDto ToDataRecordFunctionDto(this DataRecordModel record)
    {
        return new DataRecordFunctionDto
        (
            record.Id,
            record.Shard ?? string.Empty,
            record.CharacterId,
            record.PlanetId,
            record.SpeciesId,
            record.HistoricalEventId,
            record.UpdatedOn
        );
    }

    public static DataRecordModel ToDataRecordModel(this DataRecordDto record)
    {
        return new DataRecordModel()
        {
            Id = record.Id,
            Data = record.Data,
            Shard = record.Shard,
            CharacterId = record.CharacterId,
            HistoricalEventId = record.HistoricalEventId,
            PlanetId = record.PlanetId,
            SpeciesId = record.SpeciesId,
            CreatedBy = record.CreatedBy?.ToAuthorModel(),
            CreatedOn = record.CreatedOn,
            UpdatedBy = record.UpdatedBy?.ToAuthorModel(),
            UpdatedOn = record.UpdatedOn
        };
    }

    public static CreateCharacterDto ToCreateCharacterDto(this CharacterModel character)
    {
        return new CreateCharacterDto
        (
            character.GivenName,
            character.FamilyName,
            character.BirthDate,
            character.PlanetId,
            character.SpeciesIds,
            character.Aliases.Select(alias => alias.Name),
            AzureId: character.UpdatedBy?.AzureId ?? Guid.Empty
        );
    }

    public static UpdateCharacterDto ToUpdateCharacterDto(this CharacterModel character)
    {
        return new UpdateCharacterDto
        (
            character.Id,
            character.GivenName,
            character.FamilyName,
            character.BirthDate,
            character.PlanetId,
            character.SpeciesIds,
            character.Aliases.Select(alias => alias.Name),
            AzureId: character.UpdatedBy?.AzureId ?? Guid.Empty
        );
    }

    public static CharacterModel ToCharacterModel(this CharacterDto character)
    {
        return new CharacterModel()
        {
            Id = character.Id,
            GivenName = character.GivenName,
            FamilyName = character.FamilyName,
            BirthDate = character.BirthDate,
            PlanetId = character.Planet?.Id,
            Planet = character.Planet?.ToPlanetModel(),
            SpeciesIds = character.Species.Select(s => s.Id).ToList(),
            Species = character.Species.Select(s => s.ToSpeciesModel()),
            Aliases = character.Aliases.Select(alias => alias.ToAliasModel()).ToList(),
            DataRecords = character.DataRecords.Select(record => record.ToDataRecordModel()).ToList(),
            UpdatedBy = character.UpdatedBy?.ToAuthorModel(),
            UpdatedOn = character.UpdatedOn
        };
    }

    public static CharacterViewingModel ToCharacterViewingModel(this CharacterDto character)
    {
        return new CharacterViewingModel()
        {
            Id = character.Id,
            GivenName = character.GivenName,
            FamilyName = character.FamilyName,
            LatestShard = character.DataRecords.OrderByDescending(x => x.UpdatedOn).FirstOrDefault()?.Shard ?? string.Empty,
            PlanetId = character.Planet?.Id,
            Planet = character.Planet?.ToPlanetModel(),
            UpdatedOn = character.UpdatedOn
        };
    }

    public static CreatePlanetDto ToCreatePlanetDto(this PlanetModel planet)
    {
        return new CreatePlanetDto
        (
            planet.Name,
            planet.Aliases.Select(alias => alias.Name),
            AzureId: planet.UpdatedBy?.AzureId ?? Guid.Empty
        );
    }

    public static UpdatePlanetDto ToUpdatePlanetDto(this PlanetModel planet)
    {
        return new UpdatePlanetDto
        (
            planet.Id,
            planet.Name,
            planet.Aliases.Select(alias => alias.Name),
            AzureId: planet.UpdatedBy?.AzureId ?? Guid.Empty
        );
    }

    public static PlanetModel ToPlanetModel(this PlanetDto planet)
    {
        return new PlanetModel()
        {
            Id = planet.Id,
            Name = planet.Name,
            Aliases = planet.Aliases.Select(alias => alias.ToAliasModel()).ToList(),
            DataRecords = planet.DataRecords.Select(record => record.ToDataRecordModel()).ToList(),
            UpdatedBy = planet.UpdatedBy?.ToAuthorModel(),
            UpdatedOn = planet.UpdatedOn
        };
    }

    public static PlanetViewingModel ToPlanetViewingModel(this PlanetDto planet)
    {
        return new PlanetViewingModel()
        {
            Id = planet.Id,
            Name = planet.Name,
            LatestShard = planet.DataRecords.OrderByDescending(x => x.UpdatedOn).FirstOrDefault()?.Shard ?? string.Empty,
            UpdatedOn = planet.UpdatedOn
        };
    }

    public static CreateSpeciesDto ToCreateSpeciesDto(this SpeciesModel species)
    {
        return new CreateSpeciesDto
        (
            species.Name,
            species.Aliases.Select(alias => alias.Name),
            AzureId: species.UpdatedBy?.AzureId ?? Guid.Empty
        );
    }

    public static UpdateSpeciesDto ToUpdateSpeciesDto(this SpeciesModel species)
    {
        return new UpdateSpeciesDto
        (
            species.Id,
            species.Name,
            species.Aliases.Select(alias => alias.Name),
            AzureId: species.UpdatedBy?.AzureId ?? Guid.Empty
        );
    }

    public static SpeciesModel ToSpeciesModel(this SpeciesDto species)
    {
        return new SpeciesModel()
        {
            Id = species.Id,
            Name = species.Name,
            Aliases = species.Aliases.Select(alias => alias.ToAliasModel()).ToList(),
            DataRecords = species.DataRecords.Select(record => record.ToDataRecordModel()).ToList(),
            UpdatedBy = species.UpdatedBy?.ToAuthorModel(),
            UpdatedOn = species.UpdatedOn
        };
    }

    public static SpeciesViewingModel ToSpeciesViewingModel(this SpeciesDto species)
    {
        return new SpeciesViewingModel()
        {
            Id = species.Id,
            Name = species.Name,
            LatestShard = species.DataRecords.OrderByDescending(x => x.UpdatedOn).FirstOrDefault()?.Shard ?? string.Empty,
            UpdatedOn = species.UpdatedOn
        };
    }

    public static CreateHistoricalEventDto ToCreateHistoricalEventDto(this HistoricalEventModel historicalEvent)
    {
        return new CreateHistoricalEventDto
        (
            Name: historicalEvent.Name,
            DatePeriod: historicalEvent.DatePeriod,
            PlanetIds: historicalEvent.PlanetIds,
            CharacterIds: historicalEvent.CharacterIds,
            Aliases: historicalEvent.Aliases.Select(alias => alias.Name),
            AzureId: historicalEvent.UpdatedBy?.AzureId ?? Guid.Empty
        );
    }

    public static UpdateHistoricalEventDto ToUpdateHistoricalEventDto(this HistoricalEventModel historicalEvent)
    {
        return new UpdateHistoricalEventDto
        (
            Id: historicalEvent.Id,
            Name: historicalEvent.Name,
            DatePeriod: historicalEvent.DatePeriod,
            PlanetIds: historicalEvent.PlanetIds,
            CharacterIds: historicalEvent.CharacterIds,
            Aliases: historicalEvent.Aliases.Select(alias => alias.Name),
            AzureId: historicalEvent.UpdatedBy?.AzureId ?? Guid.Empty
        );
    }

    public static HistoricalEventModel ToHistoricalEventModel(this HistoricalEventDto historicalEventDto)
    {
        return new HistoricalEventModel()
        {
            Id = historicalEventDto.Id,
            Name = historicalEventDto.Name,
            DatePeriod = historicalEventDto.DatePeriod,
            PlanetIds = historicalEventDto.Planets.Select(p => p.Id).ToList(),
            Planets = historicalEventDto.Planets.Select(p => p.ToPlanetModel()),
            CharacterIds = historicalEventDto.Characters.Select(c => c.Id).ToList(),
            Characters = historicalEventDto.Characters.Select(c => c.ToCharacterModel()),
            Aliases = historicalEventDto.Aliases.Select(alias => alias.ToAliasModel()).ToList(),
            DataRecords = historicalEventDto.DataRecords.Select(record => record.ToDataRecordModel()).ToList(),
            UpdatedBy = historicalEventDto.UpdatedBy?.ToAuthorModel(),
            UpdatedOn = historicalEventDto.UpdatedOn
        };
    }

    public static HistoricalEventViewingModel ToHistoricalEventViewingModel(this HistoricalEventDto historicalEventDto)
    {
        return new HistoricalEventViewingModel()
        {
            Id = historicalEventDto.Id,
            Name = historicalEventDto.Name,
            DatePeriod = historicalEventDto.DatePeriod,
            PlanetIds = historicalEventDto.Planets.Select(p => p.Id).ToList(),
            Planets = historicalEventDto.Planets.Select(p => p.ToPlanetModel()),
            CharacterIds = historicalEventDto.Characters.Select(c => c.Id).ToList(),
            Characters = historicalEventDto.Characters.Select(c => c.ToCharacterModel()),
            LatestShard = historicalEventDto.DataRecords.OrderByDescending(x => x.UpdatedOn).FirstOrDefault()?.Shard ?? string.Empty,
            UpdatedOn = historicalEventDto.UpdatedOn
        };
    }
}
