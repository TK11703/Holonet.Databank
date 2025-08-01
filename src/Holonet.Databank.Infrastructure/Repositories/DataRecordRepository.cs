﻿using Holonet.Databank.Core.Entities;
using Holonet.Databank.Infrastructure.Data;
using Dapper;
using System.Data;
using System.Data.Common;

namespace Holonet.Databank.Infrastructure.Repositories;
public class DataRecordRepository(ISqlDataAccess dataAccess) : IDataRecordRepository
{
	private readonly ISqlDataAccess _dataAccess = dataAccess;

	public async Task<IEnumerable<DataRecord>> GetDataRecords(int? characterId = null, int? historicalEventId = null, int? planetId = null, int? speciesId = null)
	{
		return await _dataAccess.LoadDataAsync<DataRecord, dynamic>("dbo.spDataRecords_GetById", new
		{
			CharacterId = characterId,
			HistoricalEventId = historicalEventId,
			PlanetId = planetId,
			SpeciesId = speciesId
		});
	}

    public async Task<DataRecord?> GetDataRecord(int id, int? characterId = null, int? historicalEventId = null, int? planetId = null, int? speciesId = null)
    {

        var results = await _dataAccess.LoadDataAsync<DataRecord?, dynamic>("dbo.spDataRecords_Get", new
        {
            id = id,
            CharacterId = characterId,
            HistoricalEventId = historicalEventId,
            PlanetId = planetId,
            SpeciesId = speciesId
        });

        return results.FirstOrDefault();
    }

    public async Task<int?> AddDataRecord(DataRecord record)
	{
		var p = new DynamicParameters();
		p.Add(name: "@Data", record.Data);
        p.Add(name: "@Shard", record.Shard);
        p.Add(name: "@CharacterId", record.CharacterId);
		p.Add(name: "@HistoricalEventId", record.HistoricalEventId);
		p.Add(name: "@PlanetId", record.PlanetId);
		p.Add(name: "@SpeciesId", record.SpeciesId);
		p.Add(name: "@AzureAuthorId", record.CreatedBy?.AzureId);
        p.Add(name: "@NewItemId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

		await _dataAccess.SaveDataAsync("dbo.spDataRecords_Insert", p);
		var completed = p.Get<int?>("@Output");
		if (completed.HasValue && completed.Value.Equals(1))
		{
			return p.Get<int?>("@NewItemId");
		}
		else
		{
			return null;
		}
	}

    public async Task<bool> RecordExists(string shard, int? characterId = null, int? historicalEventId = null, int? planetId = null, int? speciesId = null)
    {
        var p = new DynamicParameters();
        p.Add(name: "@shard", shard);
        p.Add(name: "@CharacterId", characterId);
        p.Add(name: "@HistoricalEventId", historicalEventId);
        p.Add(name: "@PlanetId", planetId);
        p.Add(name: "@SpeciesId", speciesId);
        p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _dataAccess.SaveDataAsync("dbo.spDataRecords_Exists", p);
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

    public async Task<bool> UpdateDataRecord(DataRecord record)
    {
        var p = new DynamicParameters();
        p.Add(name: "@Id", record.Id);
        p.Add(name: "@Shard", record.Shard);
        p.Add(name: "@Data", record.Data);
		p.Add(name: "@IsProcessing", record.IsProcessing);
		p.Add(name: "@IsProcessed", record.IsProcessed);
		p.Add(name: "@SystemMessage", record.SystemMessage);
        p.Add(name: "@CharacterId", record.CharacterId);
        p.Add(name: "@HistoricalEventId", record.HistoricalEventId);
        p.Add(name: "@PlanetId", record.PlanetId);
        p.Add(name: "@SpeciesId", record.SpeciesId);
        p.Add(name: "@AzureAuthorId", record.UpdatedBy?.AzureId);
        p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _dataAccess.SaveDataAsync("dbo.spDataRecords_Update", p);
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

    public async Task<bool> DeleteDataRecord(int recordId, int? characterId = null, int? historicalEventId = null, int? planetId = null, int? speciesId = null)
	{
		var p = new DynamicParameters();
		p.Add(name: "@Id", recordId);
		p.Add(name: "@CharacterId", characterId);
		p.Add(name: "@HistoricalEventId", historicalEventId);
		p.Add(name: "@PlanetId", planetId);
		p.Add(name: "@SpeciesId", speciesId);
		p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

		await _dataAccess.SaveDataAsync("dbo.spDataRecords_Delete", p);
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

	public async Task<bool> DeleteDataRecords(int? characterId = null, int? historicalEventId = null, int? planetId = null, int? speciesId = null)
	{
		var p = new DynamicParameters();
		p.Add(name: "@CharacterId", characterId);
		p.Add(name: "@HistoricalEventId", historicalEventId);
		p.Add(name: "@PlanetId", planetId);
		p.Add(name: "@SpeciesId", speciesId);
		p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

		await _dataAccess.SaveDataAsync("dbo.spDataRecords_DeleteById", p);
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
