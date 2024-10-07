using Holonet.Databank.Core.Entities;
using Holonet.Databank.Core.Models;

namespace Holonet.Databank.Application.Services;
public interface ICharacterService
{
    Task<int> CreateCharacter(Character character);
    Task<bool> DeleteCharacter(int id);
    Task<Character?> GetCharacterById(int id);
    Task<IEnumerable<Character>> GetCharacters();
    Task<bool> CharacterExists(int id, string firstName, string lastName, int? planetId);

	Task<PageResult<Character>> GetPagedAsync(PageRequest pageRequest);
    Task<bool> UpdateCharacter(Character character);
}