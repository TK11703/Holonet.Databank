
using Holonet.Databank.Core.Entities;
using Holonet.Databank.Core.Models;
using Holonet.Databank.Infrastructure.Repositories;
using System.Data;

namespace Holonet.Databank.Application.Services;
public class CharacterService(ICharacterRepository characterRepository, IPlanetRepository planetRepository, ICharacterSpeciesRepository characterSpeciesRepository, IAuthorService authorService, IAliasRepository aliasRepository, IDataRecordService dataRecordService) : ICharacterService
{
    private readonly ICharacterRepository _characterRepository = characterRepository;
    private readonly IPlanetRepository _planetRepository = planetRepository;
    private readonly ICharacterSpeciesRepository _characterSpeciesRepository = characterSpeciesRepository;
    private readonly IAuthorService _authorService = authorService;
    private readonly IAliasRepository _aliasRepository = aliasRepository;
    private readonly IDataRecordService _dataRecordService = dataRecordService;

    public async Task<Character?> GetCharacterById(int id)
    {
        var character = await _characterRepository.GetCharacter(id);
        if (character != null)
        {
            if (character.AuthorId > 0)
            {
                var author = await _authorService.GetAuthorById(character.AuthorId, true);
                if (author != null)
                {
                    character.UpdatedBy = author;
                }
            }
            PopulateCharacter(character, true);
        }
        return character;
    }

    public async Task<bool> CharacterExists(int id, string givenName, string? familyName, int? planetId)
    {
        bool exists;
        if (planetId.HasValue)
        {
            exists = await _characterRepository.CharacterExists(id, givenName, familyName, planetId.Value);
        }
        else
        {
            exists = await _characterRepository.CharacterExists(id, givenName, familyName, default);
        }
        return exists;
    }


    public async Task<IEnumerable<Character>> GetCharacterList(bool populate = false, bool populateDataRecords = false)
    {
        if (populate)
        {
            var characters = await _characterRepository.GetCharacters();
            foreach (var character in characters)
            {
                PopulateCharacter(character, populateDataRecords);
            }
            return characters;
        }
        else
        {
            return await _characterRepository.GetCharacters();
        }
    }

    public async Task<IEnumerable<Character>> GetCharacterList(long utcTicks, bool populate = false, bool populateDataRecords = false)
    {
        if (populate)
        {
            var characters = await _characterRepository.GetCharacters(utcTicks);
            foreach (var character in characters)
            {
                PopulateCharacter(character, populateDataRecords);
            }
            return characters;
        }
        else
        {
            return await _characterRepository.GetCharacters(utcTicks);
        }
    }

    public async Task<PageResult<Character>> GetPagedAsync(PageRequest pageRequest)
    {
        var characters = await _characterRepository.GetPagedAsync(pageRequest);
        foreach (var character in characters.Collection)
        {
            PopulateCharacter(character);
        }
        return characters;
    }

    private void PopulateCharacter(Character character, bool includeDataRecords = false)
    {
        if (character.PlanetId.HasValue)
        {
            character.Planet = _planetRepository.GetPlanet(character.PlanetId.Value).Result;
        }
        character.Species = _characterSpeciesRepository.GetSpecies(character.Id).Result;
        if (character.Species.Any())
        {
            character.SpeciesIds = character.Species.Select(c => c.Id);
        }
        character.Aliases = _aliasRepository.GetAliases(characterId: character.Id).Result;
        if (character.Aliases.Any())
        {
            character.AliasIds = character.Aliases.Select(c => c.Id);
        }
        if (includeDataRecords)
        {
            character.DataRecords = _dataRecordService.GetDataRecordsById(characterId: character.Id).Result;
            if (character.DataRecords.Any())
            {
                character.DataRecordIds = character.DataRecords.Select(c => c.Id);
            }
        }
    }

    public async Task<int> CreateCharacter(Character character)
    {
        bool exists = await CharacterExists(0, character.GivenName, character.FamilyName, character.PlanetId);
        if (exists)
        {
            throw new DataException("Character already exists.");
        }
        var newId = await _characterRepository.CreateCharacter(character);
        if (newId > 0)
        {
            await _characterSpeciesRepository.AddSpecies(GetSpeciesTable(newId, character.SpeciesIds), character.UpdatedBy.AzureId);
            await _aliasRepository.AddAliases(GetAliasTable(newId, character.Aliases), character.UpdatedBy.AzureId);
        }
        return newId;
    }

    public async Task<bool> UpdateCharacter(Character character)
    {
        bool exists = await CharacterExists(character.Id, character.GivenName, character.FamilyName, character.PlanetId);

        if (exists)
        {
            throw new DataException("Character already exists.");
        }
        bool updated = _characterRepository.UpdateCharacter(character);
        if (updated)
        {
            int completedCmds = 0;
            if (await _characterSpeciesRepository.DeleteSpecies(character.Id))
            {
                completedCmds++;
                if (await _characterSpeciesRepository.AddSpecies(GetSpeciesTable(character.Id, character.SpeciesIds), character.UpdatedBy.AzureId))
                {
                    completedCmds++;
                }
            }
            if (await _aliasRepository.DeleteAliases(characterId: character.Id))
            {
                completedCmds++;
                if (await _aliasRepository.AddAliases(GetAliasTable(character.Id, character.Aliases), character.UpdatedBy.AzureId))
                {
                    completedCmds++;
                }
            }
            updated = completedCmds == 4;
        }
        return updated;
    }

    public async Task<bool> DeleteCharacter(int id)
    {
        return await _characterRepository.DeleteCharacter(id);
    }

    private static DataTable GetSpeciesTable(int characterId, IEnumerable<int> speciesIds)
    {
        DataTable dt = new();
        dt.Columns.Add("CharacterId", typeof(int));
        dt.Columns.Add("SpeciesId", typeof(int));
        foreach (var id in speciesIds)
        {
            dt.Rows.Add(characterId, id);
        }
        return dt;
    }

    private static DataTable GetAliasTable(int characterId, IEnumerable<Alias> aliases)
    {
        DataTable dt = new();
        dt.Columns.Add("AliasName", typeof(string));
        dt.Columns.Add("CharacterId", typeof(int));
        dt.Columns.Add("HistoricalEventId", typeof(int));
        dt.Columns.Add("PlanetId", typeof(int));
        dt.Columns.Add("SpeciesId", typeof(int));
        foreach (var alias in aliases)
        {
            dt.Rows.Add(alias.Name, characterId, null, null, null);
        }
        return dt;
    }
}
