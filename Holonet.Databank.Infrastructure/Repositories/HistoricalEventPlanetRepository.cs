using Holonet.Databank.Core.Entities;
using Holonet.Databank.Infrastructure.Data;
using Dapper;
using System.Data;

namespace Holonet.Databank.Infrastructure.Repositories;
public class HistoricalEventPlanetRepository(ISqlDataAccess dataAccess) : IHistoricalEventPlanetRepository
{
	private readonly ISqlDataAccess _dataAccess = dataAccess;

	public async Task<IEnumerable<Planet>> GetPlanets(int historicalEventId)
	{
		return await _dataAccess.LoadDataAsync<Planet, dynamic>("dbo.spHistoricalEventPlanets_GetByEventId", new { });
	}

	public async Task<bool> AddPlanets(DataTable historicalEventPlanets, string? createdBy = null)
	{
		var p = new DynamicParameters();
		p.Add(name: "@TableData", historicalEventPlanets.AsTableValuedParameter("HistoricalEventPlanetUDT"));
		p.Add(name: "@CreatedBy", createdBy);
		p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

		await _dataAccess.SaveDataAsync("dbo.spHistoricalEventPlanets_Insert", p);
		var completed = p.Get<int?>("@Output");
		if (completed.HasValue && completed.Value.Equals(1))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public async Task<bool> DeletePlanets(int historicalEventId)
	{
		var p = new DynamicParameters();
		p.Add(name: "@EventId", historicalEventId);
		p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

		await _dataAccess.SaveDataAsync("dbo.spHistoricalEventPlanets_DeleteByEventId", p);
		var completed = p.Get<int?>("@Output");
		if (completed.HasValue && completed.Value.Equals(1))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
