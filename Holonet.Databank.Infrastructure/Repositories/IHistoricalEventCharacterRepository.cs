using Holonet.Databank.Core.Entities;
using System.Data;

namespace Holonet.Databank.Infrastructure.Repositories;
public interface IHistoricalEventCharacterRepository
{
	Task<bool> AddCharacters(DataTable historicalEventCharacters, Guid azureId);
	Task<bool> DeleteCharacters(int historicalEventId);
	Task<IEnumerable<Character>> GetCharacters(int historicalEventId);
}