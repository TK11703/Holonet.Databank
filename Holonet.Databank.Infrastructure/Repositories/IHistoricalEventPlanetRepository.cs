using Holonet.Databank.Core.Entities;
using System.Data;

namespace Holonet.Databank.Infrastructure.Repositories;
public interface IHistoricalEventPlanetRepository
{
	Task<bool> AddPlanets(DataTable historicalEventPlanets, Guid azureId);
	Task<bool> DeletePlanets(int historicalEventId);
	Task<IEnumerable<Planet>> GetPlanets(int historicalEventId);
}