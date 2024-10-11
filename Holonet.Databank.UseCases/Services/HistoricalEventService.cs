using Holonet.Databank.Core.Entities;
using Holonet.Databank.Core.Models;
using Holonet.Databank.Infrastructure.Repositories;
using System.Data;
using System.Numerics;

namespace Holonet.Databank.Application.Services;
public class HistoricalEventService(IHistoricalEventRepository historicalEventRepository, IHistoricalEventCharacterRepository historicalEventCharacterRepository, IHistoricalEventPlanetRepository historicalEventPlanetRepository, IAuthorService authorService, IAliasRepository aliasRepository) : IHistoricalEventService
{
	private readonly IHistoricalEventRepository _historicalEventRepository = historicalEventRepository;
	private readonly IHistoricalEventCharacterRepository _historicalEventCharacterRepository = historicalEventCharacterRepository;
	private readonly IHistoricalEventPlanetRepository _historicalEventPlanetRepository = historicalEventPlanetRepository;
	private readonly IAuthorService _authorService = authorService;
	private readonly IAliasRepository _aliasRepository = aliasRepository;

	public async Task<HistoricalEvent?> GetHistoricalEventById(int id)
	{
		var item = await _historicalEventRepository.GetHistoricalEvent(id);
		if (item != null)
		{
			if (item.AuthorId > 0)
			{
				var author = await _authorService.GetAuthorById(item.AuthorId, true);
				if (author != null)
				{
					item.UpdatedBy = author;
				}
			}
			item.Characters = await _historicalEventCharacterRepository.GetCharacters(item.Id);
			if(item.Characters.Any())
            {
                item.CharacterIds = item.Characters.Select(c => c.Id);
            }
            item.Planets = await _historicalEventPlanetRepository.GetPlanets(item.Id);
            if (item.Planets.Any())
            {
                item.PlanetIds = item.Planets.Select(c => c.Id);
            }
			item.Aliases = await _aliasRepository.GetAliases(historicalEventId: item.Id);
			if (item.Aliases.Any())
			{
				item.AliasIds = item.Aliases.Select(c => c.Id);
			}
		}
		return item;
	}

	public async Task<bool> HistoricalEventExists(int id, string name)
	{
		bool exists = await _historicalEventRepository.HistoricalEventExists(id, name);
		return exists;
	}

	public async Task<IEnumerable<HistoricalEvent>> GetHistoricalEvents()
	{
		var items = await _historicalEventRepository.GetHistoricalEvents();
		foreach (var item in items)
		{
			item.Characters = await _historicalEventCharacterRepository.GetCharacters(item.Id);
            if (item.Characters.Any())
            {
                item.CharacterIds = item.Characters.Select(c => c.Id);
            }
            item.Planets = await _historicalEventPlanetRepository.GetPlanets(item.Id);
            if (item.Planets.Any())
            {
                item.PlanetIds = item.Planets.Select(c => c.Id);
            }
			item.Aliases = await _aliasRepository.GetAliases(historicalEventId: item.Id);
			if (item.Aliases.Any())
			{
				item.AliasIds = item.Aliases.Select(c => c.Id);
			}
		}
		return items;
	}

	public async Task<PageResult<HistoricalEvent>> GetPagedAsync(PageRequest pageRequest)
	{
		var items = await _historicalEventRepository.GetPagedAsync(pageRequest);
		foreach (var item in items.Collection)
		{
			item.Characters = await _historicalEventCharacterRepository.GetCharacters(item.Id);
            if (item.Characters.Any())
            {
                item.CharacterIds = item.Characters.Select(c => c.Id);
            }
            item.Planets = await _historicalEventPlanetRepository.GetPlanets(item.Id);
            if (item.Planets.Any())
            {
                item.PlanetIds = item.Planets.Select(c => c.Id);
            }
			item.Aliases = await _aliasRepository.GetAliases(historicalEventId: item.Id);
			if (item.Aliases.Any())
			{
				item.AliasIds = item.Aliases.Select(c => c.Id);
			}
		}
		return items;
	}

	public async Task<int> CreateHistoricalEvent(HistoricalEvent historicalEvent)
	{
		var exists = await _historicalEventRepository.HistoricalEventExists(0, historicalEvent.Name);
		if (exists)
		{
			throw new DataException("Historical event already exists.");
		}
		var newId = await _historicalEventRepository.CreateHistoricalEvent(historicalEvent);
		if (newId > 0)
		{
			await _historicalEventCharacterRepository.AddCharacters(GetCharacterTable(newId, historicalEvent.CharacterIds), historicalEvent.UpdatedBy.AzureId);
			await _historicalEventPlanetRepository.AddPlanets(GetPlanetTable(newId, historicalEvent.PlanetIds), historicalEvent.UpdatedBy.AzureId);
			await _aliasRepository.AddAliases(GetAliasTable(newId, historicalEvent.Aliases), historicalEvent.UpdatedBy.AzureId);
		}
		return newId;
	}

	public async Task<bool> UpdateHistoricalEvent(HistoricalEvent historicalEvent)
	{
		var exists = await _historicalEventRepository.HistoricalEventExists(historicalEvent.Id, historicalEvent.Name);
		if (exists)
		{
			throw new DataException("Historical event already exists.");
		}
		bool updated = _historicalEventRepository.UpdateHistoricalEvent(historicalEvent);
		if (updated)
		{
			int completedCmds = 0;
			if (await _historicalEventPlanetRepository.DeletePlanets(historicalEvent.Id))
			{
				completedCmds++;
				if (await _historicalEventPlanetRepository.AddPlanets(GetPlanetTable(historicalEvent.Id, historicalEvent.PlanetIds), historicalEvent.UpdatedBy.AzureId))
				{
					completedCmds++;
				}
			}
			if (await _historicalEventCharacterRepository.DeleteCharacters(historicalEvent.Id))
			{
				completedCmds++;
				if (await _historicalEventCharacterRepository.AddCharacters(GetCharacterTable(historicalEvent.Id, historicalEvent.CharacterIds), historicalEvent.UpdatedBy.AzureId))
				{
					completedCmds++;
				}
			}
			if (await _aliasRepository.DeleteAliases(historicalEventId: historicalEvent.Id))
			{
				completedCmds++;
				if (await _aliasRepository.AddAliases(GetAliasTable(historicalEvent.Id, historicalEvent.Aliases), historicalEvent.UpdatedBy.AzureId))
				{
					completedCmds++;
				}
			}
			updated = completedCmds == 6;
		}
		return updated;
	}

	public async Task<bool> DeleteHistoricalEvent(int id)
	{
		return await _historicalEventRepository.DeleteHistoricalEvent(id);
	}

	private static DataTable GetCharacterTable(int historicalEventId, IEnumerable<int> characterIds)
	{
		DataTable dt = new();
		dt.Columns.Add("HistoricalEventId", typeof(int));
		dt.Columns.Add("CharacterId", typeof(int));
		foreach (var characterId in characterIds)
		{
			dt.Rows.Add(historicalEventId, characterId);
		}
		return dt;
	}

	private static DataTable GetPlanetTable(int historicalEventId, IEnumerable<int> planetIds)
	{
		DataTable dt = new();
		dt.Columns.Add("HistoricalEventId", typeof(int));
		dt.Columns.Add("PlanetId", typeof(int));
		foreach (var planetId in planetIds)
		{
			dt.Rows.Add(historicalEventId, planetId);
		}
		return dt;
	}

	private static DataTable GetAliasTable(int historicalEventId, IEnumerable<Alias> aliases)
	{
		DataTable dt = new();
		dt.Columns.Add("AliasName", typeof(string));
		dt.Columns.Add("CharacterId", typeof(int));
		dt.Columns.Add("HistoricalEventId", typeof(int));
		dt.Columns.Add("PlanetId", typeof(int));
		dt.Columns.Add("SpeciesId", typeof(int));
		foreach (var alias in aliases)
		{
			dt.Rows.Add(alias.Name, null, historicalEventId, null, null);
		}
		return dt;
	}
}

