using Holonet.Databank.Core.Entities;
using Holonet.Databank.Core.Models;

namespace Holonet.Databank.Application.Services;
public interface ISpeciesService
{
    Task<int> CreateSpecies(Species species);
    Task<bool> DeleteSpecies(int id);
    Task<PageResult<Species>> GetPagedAsync(PageRequest pageRequest);
    Task<IEnumerable<Species>> GetSpecies();
    Task<Species?> GetSpeciesById(int id);
    Task<bool> UpdateSpecies(Species species);
}