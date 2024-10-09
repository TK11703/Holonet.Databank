
using Holonet.Databank.Core.Entities;
using Holonet.Databank.Core.Models;
using Holonet.Databank.Infrastructure.Repositories;
using System.Data;

namespace Holonet.Databank.Application.Services;
public class PlanetService(IPlanetRepository planetRepository, IAuthorService authorService) : IPlanetService
{
	private readonly IPlanetRepository _planetRepository = planetRepository;
	private readonly IAuthorService _authorService = authorService;

	public async Task<Planet?> GetPlanetById(int id)
	{
		var planet = await _planetRepository.GetPlanet(id);
		if (planet != null && planet.AuthorId > 0)
		{
			var author = await _authorService.GetAuthorById(planet.AuthorId, true);
			if (author != null)
			{
				planet.UpdatedBy = author;
			}
		}
		return planet;
	}

	public async Task<bool> PlanetExists(int id, string name)
	{
		bool exists = await _planetRepository.PlanetExists(id, name);
		return exists;
	}

	public async Task<IEnumerable<Planet>> GetPlanets()
	{
		return await _planetRepository.GetPlanets();
	}

	public async Task<PageResult<Planet>> GetPagedAsync(PageRequest pageRequest)
	{
		return await _planetRepository.GetPagedAsync(pageRequest);
	}

	public async Task<int> CreatePlanet(Planet planet)
	{
		var exists = await PlanetExists(0, planet.Name);
		if (exists)
		{
			throw new DataException("Planet already exists.");
		}
		return await _planetRepository.CreatePlanet(planet);
	}

	public async Task<bool> UpdatePlanet(Planet planet)
	{
		var exists = await PlanetExists(planet.Id, planet.Name);
		if (exists)
		{
			throw new DataException("Planet already exists.");
		}
		return _planetRepository.UpdatePlanet(planet);
	}

	public async Task<bool> DeletePlanet(int id)
	{
		return await _planetRepository.DeletePlanet(id);
	}
}
