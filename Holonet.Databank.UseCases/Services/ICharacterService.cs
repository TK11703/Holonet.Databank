using Holonet.Databank.Core.Entities;
using Holonet.Databank.Core.Models;

namespace Holonet.Databank.Application.Services;
public interface ICharacterService
{
    Task<bool> CharacterExists(int id, string givenName, string? familyName, int? planetId);
    Task<int> CreateCharacter(Character character);
    Task<bool> DeleteCharacter(int id);
    Task<Character?> GetCharacterById(int id);
    Task<IEnumerable<Character>> GetCharacterList(bool populate = false, bool populateDataRecords = false);
    Task<IEnumerable<Character>> GetCharacterList(long utcTicks, bool populate = false, bool populateDataRecords = false);
    Task<PageResult<Character>> GetPagedAsync(PageRequest pageRequest);
    Task<bool> UpdateCharacter(Character character);
}