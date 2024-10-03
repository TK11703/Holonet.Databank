using Holonet.Databank.Core.Entities;
using Holonet.Databank.Core.Models;

namespace Holonet.Databank.Infrastructure.Repositories;
public interface ICharacterRepository
{
	Task<bool> CharacterExists(int id, string firstName, string lastName, int? planetId);
	Task<int> CreateCharacter(Character itemModel);
	Task<bool> DeleteCharacter(int id);
	Task<Character?> GetCharacter(int id);
	Task<IEnumerable<Character>> GetCharacters();
	Task<PageResult<Character>> GetPagedAsync(PageRequest pageRequest);
	bool UpdateCharacter(Character itemModel);
}