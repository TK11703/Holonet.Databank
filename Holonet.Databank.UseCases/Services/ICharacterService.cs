using Holonet.Databank.Core.Entities;
using Holonet.Databank.Core.Models;

namespace Holonet.Databank.Application.Services;
public interface ICharacterService
{
    Task<int> CreateCharacter(Character character, string? createdBy = null);
    Task<bool> DeleteCharacter(int id);
    Task<Character?> GetCharacterById(int id);
    Task<IEnumerable<Character>> GetCharacters();
    Task<PageResult<Character>> GetPagedAsync(PageRequest pageRequest);
    Task<bool> UpdateCharacter(Character character, string? updatedBy = null);
}