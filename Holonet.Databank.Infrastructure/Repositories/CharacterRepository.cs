﻿using Holonet.Databank.Core.Entities;
using Holonet.Databank.Infrastructure.Data;
using Dapper;
using System.Data;
using Holonet.Databank.Core.Models;

namespace Holonet.Databank.Infrastructure.Repositories;
public class CharacterRepository(ISqlDataAccess dataAccess) : ICharacterRepository
{
	private readonly ISqlDataAccess _dataAccess = dataAccess;

	public async Task<IEnumerable<Character>> GetCharacters()
	{
		return await _dataAccess.LoadDataAsync<Character, dynamic>("dbo.spCharacters_GetAll", new { });
	}

	public async Task<PageResult<Character>> GetPagedAsync(PageRequest pageRequest)
	{
		PageResult<Character> results = new PageResult<Character>(pageRequest.PageSize, pageRequest.Start);
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
		var characters = await _dataAccess.LoadDataAsync<Character, dynamic>("dbo.spCharacters_GetPaged", p);
		if (characters != null)
		{
			results.ItemCount = p.Get<int>("@Total");
			results.Collection = characters;
			return results;
		}

		return new PageResult<Character>();
	}

	public async Task<Character?> GetCharacter(int id)
	{
		var results = await _dataAccess.LoadDataAsync<Character, dynamic>("dbo.spCharacters_Get", new { Id = id });

		return results.FirstOrDefault();
	}

	public async Task<int> CreateCharacter(Character itemModel, string? createdBy = null)
	{
		var p = new DynamicParameters();
		p.Add(name: "@FirstName", itemModel.FirstName);
		p.Add(name: "@LastName", itemModel.LastName);
		p.Add(name: "@Description", itemModel.Description);
		p.Add(name: "@BirthDate", itemModel.BirthDate);
		p.Add(name: "@Shard", itemModel.Shard);
		p.Add(name: "@PlanetId", itemModel.PlanetId);
		p.Add(name: "@CreatedBy", createdBy);
		p.Add(name: "@Id", value: 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
		p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

		await _dataAccess.SaveDataAsync("dbo.spCharacters_Insert", p);
		var newId = p.Get<int?>("@Id");
		return newId ?? 0;
	}

	public bool UpdateCharacter(Character itemModel, string? updatedBy = null)
	{
		var p = new DynamicParameters();
		p.Add(name: "@Id", itemModel.Id);
		p.Add(name: "@FirstName", itemModel.FirstName);
		p.Add(name: "@LastName", itemModel.LastName);
		p.Add(name: "@Description", itemModel.Description);
		p.Add(name: "@BirthDate", itemModel.BirthDate);
		p.Add(name: "@Shard", itemModel.Shard);
		p.Add(name: "@PlanetId", itemModel.PlanetId);
		p.Add(name: "@UpdatedBy", updatedBy);
		p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

		_dataAccess.SaveData<dynamic>("dbo.spCharacters_Update", p);
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

	public async Task<bool> CharacterExists(int id, string firstName, string lastName, int? planetId)
	{
		var p = new DynamicParameters();
		p.Add(name: "@Id", id);
		p.Add(name: "@FirstName", firstName);
		p.Add(name: "@LastName", lastName);
		p.Add(name: "@PlanetId", planetId);
		p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
		await _dataAccess.LoadDataAsync<Character, dynamic>("dbo.spCharacters_Exists", p);
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

	public async Task<bool> DeleteCharacter(int id)
	{
		var p = new DynamicParameters();
		p.Add(name: "@Id", id);
		p.Add(name: "@Output", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

		await _dataAccess.SaveDataAsync("dbo.spCharacters_Delete", p);
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