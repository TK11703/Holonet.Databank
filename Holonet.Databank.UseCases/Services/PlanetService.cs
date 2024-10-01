
using Holonet.Databank.Core.Entities;
using Holonet.Databank.Core.Models;
using Holonet.Databank.Infrastructure.Repositories;
using System.Data;

namespace Holonet.Databank.Application.Services;
public class PlanetService(IPlanetRepository planetRepository) : IPlanetService
{
	private readonly IPlanetRepository _planetRepository = planetRepository;

	public async Task<Planet?> GetPlanetById(int id)
	{
		return await _planetRepository.GetPlanet(id);
	}

	public async Task<IEnumerable<Planet>> GetPlanets()
	{
		return await _planetRepository.GetPlanets();
	}

	public async Task<PageResult<Planet>> GetPagedAsync(PageRequest pageRequest)
	{
		return await _planetRepository.GetPagedAsync(pageRequest);
	}

	public async Task<int> CreatePlanet(Planet planet, string? createdBy = null)
	{
		var exists = await _planetRepository.PlanetExists(0, planet.Name);
		if (exists)
		{
			throw new DataException("Planet already exists.");
		}
		return await _planetRepository.CreatePlanet(planet, createdBy);
	}

	public async Task<bool> UpdatePlanet(Planet planet, string? updatedBy = null)
	{
		var exists = await _planetRepository.PlanetExists(planet.Id, planet.Name);
		if (exists)
		{
			throw new DataException("Planet already exists.");
		}
		return _planetRepository.UpdatePlanet(planet, updatedBy);
	}

	public async Task<bool> DeletePlanet(int id)
	{
		return await _planetRepository.DeletePlanet(id);
	}
}
