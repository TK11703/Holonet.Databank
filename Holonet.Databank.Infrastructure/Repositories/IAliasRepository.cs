using Holonet.Databank.Core.Entities;
using System.Data;

namespace Holonet.Databank.Infrastructure.Repositories;
public interface IAliasRepository
{
	Task<bool> AddAliases(DataTable aliases, Guid azureId);
	Task<bool> DeleteAliases(int? characterId = null, int? historicalEventId = null, int? planetId = null, int? speciesId = null);
	Task<IEnumerable<Alias>> GetAliases(int? characterId = null, int? historicalEventId = null, int? planetId = null, int? speciesId = null);
}