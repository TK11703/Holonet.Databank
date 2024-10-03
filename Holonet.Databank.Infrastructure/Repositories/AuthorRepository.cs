using Holonet.Databank.Core.Entities;
using Holonet.Databank.Infrastructure.Data;
using Dapper;
using System.Data;

namespace Holonet.Databank.Infrastructure.Repositories;
public class AuthorRepository(ISqlDataAccess dataAccess) : IAuthorRepository
{
	private readonly ISqlDataAccess _dataAccess = dataAccess;

	public async Task<Author?> GetAuthor(Guid azureId)
	{
		var results = await _dataAccess.LoadDataAsync<Author, dynamic>("dbo.spAuthors_GetByAzureId", new { AzureId = azureId });

		return results.FirstOrDefault();
	}

	public async Task<int> CreateAuthor(Author itemModel)
	{
		var p = new DynamicParameters();
		p.Add(name: "@AzureId", itemModel.AzureId);
		p.Add(name: "@DisplayName", itemModel.DisplayName);
		p.Add(name: "@Email", itemModel.Email);
		p.Add(name: "@Id", value: 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
		p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

		await _dataAccess.SaveDataAsync("dbo.spAuthors_Insert", p);
		var newId = p.Get<int?>("@Id");
		return newId ?? 0;
	}

	public bool UpdateAuthor(Author itemModel)
	{
		var p = new DynamicParameters();
		p.Add(name: "@Id", itemModel.Id);
		p.Add(name: "@AzureId", itemModel.AzureId);
		p.Add(name: "@DisplayName", itemModel.DisplayName);
		p.Add(name: "@Email", itemModel.Email);
		p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

		_dataAccess.SaveData<dynamic>("dbo.spAuthors_Update", p);
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
