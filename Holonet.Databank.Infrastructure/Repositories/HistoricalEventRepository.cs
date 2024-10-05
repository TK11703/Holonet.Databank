using Holonet.Databank.Core.Entities;
using Holonet.Databank.Infrastructure.Data;
using Dapper;
using System.Data;
using Holonet.Databank.Core.Models;

namespace Holonet.Databank.Infrastructure.Repositories;
public class HistoricalEventRepository(ISqlDataAccess dataAccess) : IHistoricalEventRepository
{
	private readonly ISqlDataAccess _dataAccess = dataAccess;

	public async Task<IEnumerable<HistoricalEvent>> GetHistoricalEvents()
	{
		return await _dataAccess.LoadDataAsync<HistoricalEvent, dynamic>("dbo.spHistoricalEvents_GetAll", new { });
	}

	public async Task<PageResult<HistoricalEvent>> GetPagedAsync(PageRequest pageRequest)
	{
		PageResult<HistoricalEvent> results = new PageResult<HistoricalEvent>(pageRequest.PageSize, pageRequest.Start);
		var p = new DynamicParameters();
		p.Add(name: "@SortBy", pageRequest.SortBy);
		p.Add(name: "@SortOrder", pageRequest.SortDirection);
		p.Add(name: "@PageSize", pageRequest.PageSize);
		p.Add(name: "@Start", pageRequest.Start);
		if (!string.IsNullOrEmpty(pageRequest.Filter))
		{
			p.Add(name: "@Search", pageRequest.Filter);
		}
		if (pageRequest.BeginDate.HasValue)
		{
			p.Add(name: "@Begin", pageRequest.BeginDate.Value.Date);
		}
		if (pageRequest.EndDate.HasValue)
		{
			p.Add(name: "@End", pageRequest.EndDate.Value.Date);
		}
		p.Add(name: "@Total", value: 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
		var events = await _dataAccess.LoadDataAsync<HistoricalEvent, dynamic>("dbo.spHistoricalEvents_GetPaged", p);
		if (events != null)
		{
			results.ItemCount = p.Get<int>("@Total");
			results.Collection = events;
			return results;
		}

		return new PageResult<HistoricalEvent>();
	}

	public async Task<HistoricalEvent?> GetHistoricalEvent(int id)
	{
		var results = await _dataAccess.LoadDataAsync<HistoricalEvent, dynamic>("dbo.spHistoricalEvents_Get", new { Id = id });

		return results.FirstOrDefault();
	}

	public async Task<int> CreateHistoricalEvent(HistoricalEvent itemModel)
	{
		var p = new DynamicParameters();
		p.Add(name: "@Name", itemModel.Name);
		p.Add(name: "@Description", itemModel.Description);
		p.Add(name: "@Shard", itemModel.Shard);
		p.Add(name: "@DatePeriod", itemModel.DatePeriod);
		p.Add(name: "@AzureAuthorId", itemModel.UpdatedBy.AzureId);
		p.Add(name: "@Id", value: 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
		p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

		await _dataAccess.SaveDataAsync("dbo.spHistoricalEvents_Insert", p);
		var newId = p.Get<int?>("@Id");
		return newId ?? 0;
	}

	public bool UpdateHistoricalEvent(HistoricalEvent itemModel)
	{
		var p = new DynamicParameters();
		p.Add(name: "@Id", itemModel.Id);
		p.Add(name: "@Name", itemModel.Name);
		p.Add(name: "@Description", itemModel.Description);
		p.Add(name: "@Shard", itemModel.Shard);
		p.Add(name: "@DatePeriod", itemModel.DatePeriod);
		p.Add(name: "@AzureAuthorId", itemModel.UpdatedBy.AzureId);
		p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

		_dataAccess.SaveData<dynamic>("dbo.spHistoricalEvents_Update", p);
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

	public async Task<bool> HistoricalEventExists(int id, string name)
	{
		var p = new DynamicParameters();
		p.Add(name: "@Id", id);
		p.Add(name: "@Name", name);
		p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
		await _dataAccess.LoadDataAsync<HistoricalEvent, dynamic>("dbo.spHistoricalEvents_Exists", p);
		var exists = p.Get<int?>("@Output");
		if (exists.HasValue && exists.Value.Equals(1))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public async Task<bool> DeleteHistoricalEvent(int id)
	{
		var p = new DynamicParameters();
		p.Add(name: "@Id", id);
		p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

		await _dataAccess.SaveDataAsync("dbo.spHistoricalEvents_Delete", p);
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
