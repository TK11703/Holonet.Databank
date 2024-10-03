using Holonet.Databank.Core.Entities;
using Holonet.Databank.Core.Models;

namespace Holonet.Databank.Infrastructure.Repositories;
public interface ISpeciesRepository
{
	Task<int> CreateSpecies(Species itemModel);
	Task<bool> DeleteSpecies(int id);
	Task<PageResult<Species>> GetPagedAsync(PageRequest pageRequest);
	Task<IEnumerable<Species>> GetSpecies();
	Task<Species?> GetSpecies(int id);
	Task<bool> SpeciesExists(int id, string name);
	bool UpdateSpecies(Species itemModel);
}