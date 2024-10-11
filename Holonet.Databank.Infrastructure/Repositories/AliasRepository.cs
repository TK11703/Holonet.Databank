using Holonet.Databank.Core.Entities;
using Holonet.Databank.Infrastructure.Data;
using Dapper;
using System.Data;

namespace Holonet.Databank.Infrastructure.Repositories;
public class AliasRepository(ISqlDataAccess dataAccess) : IAliasRepository
{
	private readonly ISqlDataAccess _dataAccess = dataAccess;

	public async Task<IEnumerable<Alias>> GetAliases(int? characterId = null, int? historicalEventId = null, int ? planetId = null, int? speciesId = null)
	{
		return await _dataAccess.LoadDataAsync<Alias, dynamic>("dbo.spAliases_GetById", new { 
			CharacterId = characterId, HistoricalEventId=historicalEventId, PlanetId = planetId, SpeciesId = speciesId 
		});
	}

	public async Task<bool> AddAliases(DataTable aliases, Guid azureId)
	{
		var p = new DynamicParameters();
		p.Add(name: "@TableData", aliases.AsTableValuedParameter("AliasesUDT"));
		p.Add(name: "@AzureAuthorId", azureId);
		p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

		await _dataAccess.SaveDataAsync("dbo.spAliases_Insert", p);
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

	public async Task<bool> DeleteAliases(int? characterId = null, int? historicalEventId = null, int? planetId = null, int? speciesId = null)
	{
		var p = new DynamicParameters();
		p.Add(name: "@CharacterId", characterId);
		p.Add(name: "@HistoricalEventId", historicalEventId);
		p.Add(name: "@PlanetId", planetId);
		p.Add(name: "@SpeciesId", speciesId);
		p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

		await _dataAccess.SaveDataAsync("dbo.spAliases_DeleteById", p);
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
