
using Holonet.Databank.Core.Entities;
using Holonet.Databank.Core.Models;
using Holonet.Databank.Infrastructure.Repositories;
using System.Data;

namespace Holonet.Databank.Application.Services;
public class SpeciesService(ISpeciesRepository speciesRepository) : ISpeciesService
{
    private readonly ISpeciesRepository _speciesRepository = speciesRepository;

    public async Task<Species?> GetSpeciesById(int id)
    {
        return await _speciesRepository.GetSpecies(id);
    }

    public async Task<IEnumerable<Species>> GetSpecies()
    {
        return await _speciesRepository.GetSpecies();
    }

    public async Task<PageResult<Species>> GetPagedAsync(PageRequest pageRequest)
    {
        return await _speciesRepository.GetPagedAsync(pageRequest);
    }

    public async Task<int> CreateSpecies(Species species)
    {
        var exists = await _speciesRepository.SpeciesExists(0, species.Name);
        if (exists)
        {
            throw new DataException("Species already exists.");
        }
        return await _speciesRepository.CreateSpecies(species);
    }

    public async Task<bool> UpdateSpecies(Species species)
    {
        var exists = await _speciesRepository.SpeciesExists(species.Id, species.Name);
        if (exists)
        {
            throw new DataException("Species already exists.");
        }
        return _speciesRepository.UpdateSpecies(species);
    }

    public async Task<bool> DeleteSpecies(int id)
    {
        return await _speciesRepository.DeleteSpecies(id);
    }
}
