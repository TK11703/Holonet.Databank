﻿
using Holonet.Databank.Core.Entities;
using Holonet.Databank.Core.Models;
using Holonet.Databank.Infrastructure.Repositories;
using System.Data;

namespace Holonet.Databank.Application.Services;
public class CharacterService(ICharacterRepository characterRepository, IPlanetRepository planetRepository, ICharacterSpeciesRepository characterSpeciesRepository) : ICharacterService
{
    private readonly ICharacterRepository _characterRepository = characterRepository;
    private readonly IPlanetRepository _planetRepository = planetRepository;
    private readonly ICharacterSpeciesRepository _characterSpeciesRepository = characterSpeciesRepository;

    public async Task<Character?> GetCharacterById(int id)
    {
        var character = await _characterRepository.GetCharacter(id);
        if (character != null)
        {
            if (character.PlanetId.HasValue)
            {
                character.Planet = await _planetRepository.GetPlanet(character.PlanetId.Value);
            }
            character.Species = await _characterSpeciesRepository.GetSpecies(character.Id);
            if (character.Species.Any())
            {
                character.SpeciesIds = character.Species.Select(c => c.Id);
            }
        }
        return character;
    }

    public async Task<IEnumerable<Character>> GetCharacters()
    {
        var characters = await _characterRepository.GetCharacters();
        foreach (var character in characters)
        {
            if (character.PlanetId.HasValue)
            {
                character.Planet = await _planetRepository.GetPlanet(character.PlanetId.Value);
            }
            character.Species = await _characterSpeciesRepository.GetSpecies(character.Id);
            if (character.Species.Any())
            {
                character.SpeciesIds = character.Species.Select(c => c.Id);
            }
        }
        return characters;
    }

    public async Task<PageResult<Character>> GetPagedAsync(PageRequest pageRequest)
    {
        var characters = await _characterRepository.GetPagedAsync(pageRequest);
        foreach (var character in characters.Collection)
        {
            if (character.PlanetId.HasValue)
            {
                character.Planet = await _planetRepository.GetPlanet(character.PlanetId.Value);
            }
            character.Species = await _characterSpeciesRepository.GetSpecies(character.Id);
            if (character.Species.Any())
            {
                character.SpeciesIds = character.Species.Select(c => c.Id);
            }
        }
        return characters;
    }

    public async Task<int> CreateCharacter(Character character)
    {
        bool exists;
        if (character.PlanetId.HasValue)
        {
            exists = await _characterRepository.CharacterExists(0, character.FirstName, character.LastName, character.PlanetId.Value);
        }
        else
        {
            exists = await _characterRepository.CharacterExists(0, character.FirstName, character.LastName, default);
        }
        if (exists)
        {
            throw new DataException("Character already exists.");
        }
        return await _characterRepository.CreateCharacter(character);
    }

    public async Task<bool> UpdateCharacter(Character character)
    {
        bool exists;
        if (character.PlanetId.HasValue)
        {
            exists = await _characterRepository.CharacterExists(character.Id, character.FirstName, character.LastName, character.PlanetId.Value);
        }
        else
        {
            exists = await _characterRepository.CharacterExists(character.Id, character.FirstName, character.LastName, default);
        }
        if (exists)
        {
            throw new DataException("Character already exists.");
        }
        return _characterRepository.UpdateCharacter(character);
    }

    public async Task<bool> DeleteCharacter(int id)
    {
        return await _characterRepository.DeleteCharacter(id);
    }
}
