using Holonet.Databank.Core.Entities;
using Holonet.Databank.Infrastructure.Data;
using Dapper;
using System.Data;

namespace Holonet.Databank.Infrastructure.Repositories;
public class CharacterSpeciesRepository(ISqlDataAccess dataAccess) : ICharacterSpeciesRepository
{
	private readonly ISqlDataAccess _dataAccess = dataAccess;

	public async Task<IEnumerable<Species>> GetSpecies(int characterId)
	{
		return await _dataAccess.LoadDataAsync<Species, dynamic>("dbo.spCharacterSpecies_GetByCharacterId", new { });
	}

	public async Task<bool> AddSpecies(DataTable characterSpecies, Guid azureId)
	{
		var p = new DynamicParameters();
		p.Add(name: "@TableData", characterSpecies.AsTableValuedParameter("CharacterSpeciesUDT"));
		p.Add(name: "@AzureAuthorId", azureId);
		p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

		await _dataAccess.SaveDataAsync("dbo.spCharacterSpecies_Insert", p);
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

	public async Task<bool> DeleteSpecies(int characterId)
	{
		var p = new DynamicParameters();
		p.Add(name: "@CharacterId", characterId);
		p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

		await _dataAccess.SaveDataAsync("dbo.spCharacterSpecies_DeleteByCharacterId", p);
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
