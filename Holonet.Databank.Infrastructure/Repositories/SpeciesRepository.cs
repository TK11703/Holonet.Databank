﻿using Holonet.Databank.Core.Entities;
using Holonet.Databank.Infrastructure.Data;
using Dapper;
using System.Data;
using Holonet.Databank.Core.Models;

namespace Holonet.Databank.Infrastructure.Repositories;
public class SpeciesRepository(ISqlDataAccess dataAccess) : ISpeciesRepository
{
    private readonly ISqlDataAccess _dataAccess = dataAccess;

    public async Task<IEnumerable<Species>> GetSpecies()
    {
        return await _dataAccess.LoadDataAsync<Species, dynamic>("dbo.spSpecies_GetAll", new { });
    }

    public async Task<Species?> GetSpecies(int id)
    {
        var results = await _dataAccess.LoadDataAsync<Species, dynamic>("dbo.spSpecies_Get", new { Id = id });

        return results.FirstOrDefault();
    }

    public async Task<PageResult<Species>> GetPagedAsync(PageRequest pageRequest)
    {
        PageResult<Species> results = new PageResult<Species>(pageRequest.PageSize, pageRequest.Start);
        var p = new DynamicParameters();
        p.Add(name: "@SortBy", pageRequest.SortBy);
        p.Add(name: "@SortOrder", pageRequest.SortDirection);
        p.Add(name: "@PageSize", pageRequest.PageSize);
        p.Add(name: "@Start", pageRequest.Start);
        p.Add(name: "@PageSize", pageRequest.PageSize);
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
        var planets = await _dataAccess.LoadDataAsync<Species, dynamic>("dbo.spSpecies_GetPaged", p);
        if (planets != null)
        {
            results.ItemCount = p.Get<int>("@Total");
            results.Collection = planets;
            return results;
        }

        return new PageResult<Species>();
    }

    public async Task<int> CreateSpecies(Species itemModel, string? createdBy = null)
    {
        var p = new DynamicParameters();
        p.Add(name: "@Name", itemModel.Name);
        p.Add(name: "@Description", itemModel.Description);
        p.Add(name: "@Shard", itemModel.Shard);
        p.Add(name: "@CreatedBy", createdBy);
        p.Add(name: "@Id", value: 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _dataAccess.SaveDataAsync("dbo.spSpecies_Insert", p);
        var newId = p.Get<int?>("@Id");
        return newId ?? 0;
    }

    public bool UpdateSpecies(Species itemModel, string? updatedBy = null)
    {
        var p = new DynamicParameters();
        p.Add(name: "@Id", itemModel.Id);
        p.Add(name: "@Name", itemModel.Name);
        p.Add(name: "@Description", itemModel.Description);
        p.Add(name: "@Shard", itemModel.Shard);
        p.Add(name: "@UpdatedBy", updatedBy);
        p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        _dataAccess.SaveData<dynamic>("dbo.spSpecies_Update", p);
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

    public async Task<bool> SpeciesExists(int id, string name)
    {
        var p = new DynamicParameters();
        p.Add(name: "@Id", id);
        p.Add(name: "@Name", name);
        p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        await _dataAccess.LoadDataAsync<Species, dynamic>("dbo.spSpecies_Exists", p);
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

    public async Task<bool> DeleteSpecies(int id)
    {
        var p = new DynamicParameters();
        p.Add(name: "@Id", id);
        p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        await _dataAccess.SaveDataAsync("dbo.spSpecies_Delete", p);
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
