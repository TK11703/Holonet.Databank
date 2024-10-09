
using Holonet.Databank.Core.Entities;
using Holonet.Databank.Core.Models;
using Holonet.Databank.Infrastructure.Repositories;
using System.Data;

namespace Holonet.Databank.Application.Services;
public class CharacterService(ICharacterRepository characterRepository, IPlanetRepository planetRepository, ICharacterSpeciesRepository characterSpeciesRepository, IAuthorService authorService) : ICharacterService
{
    private readonly ICharacterRepository _characterRepository = characterRepository;
    private readonly IPlanetRepository _planetRepository = planetRepository;
    private readonly ICharacterSpeciesRepository _characterSpeciesRepository = characterSpeciesRepository;
	private readonly IAuthorService _authorService = authorService;

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

	public async Task<bool> CharacterExists(int id, string firstName, string lastName, int? planetId)
	{
		bool exists;
		if (planetId.HasValue)
		{
			exists = await _characterRepository.CharacterExists(id, firstName, lastName, planetId.Value);
		}
		else
		{
			exists = await _characterRepository.CharacterExists(id, firstName, lastName, default);
		}
        return exists;
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
        bool exists = await CharacterExists(0, character.FirstName, character.LastName, character.PlanetId);
        if (exists)
        {
            throw new DataException("Character already exists.");
        }
        var newId = await _characterRepository.CreateCharacter(character);
		if (newId > 0)
        {
			await _characterSpeciesRepository.AddSpecies(GetSpeciesTable(newId, character.SpeciesIds), character.UpdatedBy.AzureId);
		}
		return newId;
	}

    public async Task<bool> UpdateCharacter(Character character)
    {
        bool exists = await CharacterExists(character.Id, character.FirstName, character.LastName, character.PlanetId);
		
        if (exists)
        {
            throw new DataException("Character already exists.");
        }
        bool updated = _characterRepository.UpdateCharacter(character);
        if(updated)
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
			updated = completedCmds == 2;
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
}
