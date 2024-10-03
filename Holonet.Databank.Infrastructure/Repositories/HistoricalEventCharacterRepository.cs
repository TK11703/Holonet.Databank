using Holonet.Databank.Core.Entities;
using Holonet.Databank.Infrastructure.Data;
using Dapper;
using System.Data;

namespace Holonet.Databank.Infrastructure.Repositories;
public class HistoricalEventCharacterRepository(ISqlDataAccess dataAccess) : IHistoricalEventCharacterRepository
{
	private readonly ISqlDataAccess _dataAccess = dataAccess;

	public async Task<IEnumerable<Character>> GetCharacters(int historicalEventId)
	{
		return await _dataAccess.LoadDataAsync<Character, dynamic>("dbo.spHistoricalEventCharacters_GetByEventId", new { });
	}

	public async Task<bool> AddCharacters(DataTable historicalEventCharacters, Guid azureId)
	{
		var p = new DynamicParameters();
		p.Add(name: "@TableData", historicalEventCharacters.AsTableValuedParameter("HistoricalEventCharacterUDT"));
		p.Add(name: "@AzureAuthorId", azureId);
		p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

		await _dataAccess.SaveDataAsync("dbo.spHistoricalEventCharacters_Insert", p);
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

	public async Task<bool> DeleteCharacters(int historicalEventId)
	{
		var p = new DynamicParameters();
		p.Add(name: "@EventId", historicalEventId);
		p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

		await _dataAccess.SaveDataAsync("dbo.spHistoricalEventCharacters_DeleteByEventId", p);
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
