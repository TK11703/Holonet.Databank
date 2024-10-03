using Holonet.Databank.Core.Entities;
using System.Data;

namespace Holonet.Databank.Infrastructure.Repositories;
public interface ICharacterSpeciesRepository
{
	Task<bool> AddSpecies(DataTable characterSpecies, Guid azureId);
	Task<bool> DeleteSpecies(int characterId);
	Task<IEnumerable<Species>> GetSpecies(int characterId);
}