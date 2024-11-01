
using Holonet.Databank.Core.Entities;
using Holonet.Databank.Core.Models;
using Holonet.Databank.Infrastructure.Repositories;
using System.Data;

namespace Holonet.Databank.Application.Services;
public class PlanetService(IPlanetRepository planetRepository, IAuthorService authorService, IAliasRepository aliasRepository, IDataRecordService dataRecordService) : IPlanetService
{
	private readonly IPlanetRepository _planetRepository = planetRepository;
	private readonly IAuthorService _authorService = authorService;
	private readonly IAliasRepository _aliasRepository = aliasRepository;
	private readonly IDataRecordService _dataRecordService = dataRecordService;


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

			planet.Aliases = await _aliasRepository.GetAliases(planetId: planet.Id);
			if (planet.Aliases.Any())
			{
				planet.AliasIds = planet.Aliases.Select(c => c.Id);
			}

			planet.DataRecords = await _dataRecordService.GetDataRecordsById(planetId: planet.Id);
			if (planet.DataRecords.Any())
			{
				planet.DataRecordIds = planet.DataRecords.Select(c => c.Id);
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
		int newId = await _planetRepository.CreatePlanet(planet);
		if (newId > 0)
		{
			await _aliasRepository.AddAliases(GetAliasTable(newId, planet.Aliases), planet.UpdatedBy.AzureId);
		}
		return newId;
	}

	public async Task<bool> UpdatePlanet(Planet planet)
	{
		var exists = await PlanetExists(planet.Id, planet.Name);
		if (exists)
		{
			throw new DataException("Planet already exists.");
		}
		bool updated = _planetRepository.UpdatePlanet(planet);
		if(updated)
		{
			int completedCmds = 0;
			if (await _aliasRepository.DeleteAliases(planetId: planet.Id))
			{
				completedCmds++;
				if (await _aliasRepository.AddAliases(GetAliasTable(planet.Id, planet.Aliases), planet.UpdatedBy.AzureId))
				{
					completedCmds++;
				}
			}
			updated = completedCmds == 2;
		}
		return updated;
	}

	public async Task<bool> DeletePlanet(int id)
	{
		return await _planetRepository.DeletePlanet(id);
	}

	private static DataTable GetAliasTable(int planetId, IEnumerable<Alias> aliases)
	{
		DataTable dt = new();
		dt.Columns.Add("AliasName", typeof(string));
		dt.Columns.Add("CharacterId", typeof(int));
		dt.Columns.Add("HistoricalEventId", typeof(int));
		dt.Columns.Add("PlanetId", typeof(int));
		dt.Columns.Add("SpeciesId", typeof(int));
		foreach (var alias in aliases)
		{
			dt.Rows.Add(alias.Name, null, null, planetId, null);
		}
		return dt;
	}
}
