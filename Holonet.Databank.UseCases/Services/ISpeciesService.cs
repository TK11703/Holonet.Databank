using Holonet.Databank.Core.Entities;
using Holonet.Databank.Core.Models;

namespace Holonet.Databank.Application.Services;
public interface ISpeciesService
{
    Task<int> CreateSpecies(Species species);
    Task<bool> DeleteSpecies(int id);
    Task<PageResult<Species>> GetPagedAsync(PageRequest pageRequest);
    Task<IEnumerable<Species>> GetSpecies(bool populate = false, bool populateDataRecords = false);
    Task<IEnumerable<Species>> GetSpecies(long utcTicks, bool populate = false, bool populateDataRecords = false);
    Task<Species?> GetSpeciesById(int id);
    Task<bool> SpeciesExists(int id, string name);
    Task<bool> UpdateSpecies(Species species);
}