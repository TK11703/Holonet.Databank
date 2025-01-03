﻿using Holonet.Databank.Core.Entities;
using Holonet.Databank.Core.Models;

namespace Holonet.Databank.Infrastructure.Repositories;
public interface IPlanetRepository
{
	Task<int> CreatePlanet(Planet itemModel);
	Task<bool> DeletePlanet(int id);
	Task<PageResult<Planet>> GetPagedAsync(PageRequest pageRequest);
	Task<Planet?> GetPlanet(int id);
	Task<IEnumerable<Planet>> GetPlanets();

    Task<IEnumerable<Planet>> GetPlanets(long utcTicks);
    Task<bool> PlanetExists(int id, string name);
	bool UpdatePlanet(Planet itemModel);
}