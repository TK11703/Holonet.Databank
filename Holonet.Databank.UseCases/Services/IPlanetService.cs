using Holonet.Databank.Core.Entities;
using Holonet.Databank.Core.Models;

namespace Holonet.Databank.Application.Services;
public interface IPlanetService
{
	Task<int> CreatePlanet(Planet planet);
	Task<bool> DeletePlanet(int id);
	Task<PageResult<Planet>> GetPagedAsync(PageRequest pageRequest);
	Task<Planet?> GetPlanetById(int id);
	Task<IEnumerable<Planet>> GetPlanets();
	Task<bool> UpdatePlanet(Planet planet);
}