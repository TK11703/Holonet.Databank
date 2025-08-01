﻿using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Models;
using Holonet.Databank.Web.Models;
using Holonet.Databank.Web.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace Holonet.Databank.Web.Clients;

public sealed class HistoricalEventClient : ClientBase
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<HistoricalEventClient> _logger;

	public HistoricalEventClient(HttpClient httpClient, ILogger<HistoricalEventClient> logger, ITokenAcquisition tokenAcquisition, IOptions<AppSettings> options) : base(tokenAcquisition, options)
	{
		_httpClient = httpClient;
		_logger = logger;
		PerformClientChecks();
	}

	private void PerformClientChecks()
	{
		if (_httpClient.BaseAddress == null)
		{
			_logger.LogError("BaseAddress of HistoricalEventClient cannot be null.");
		}
	}

	public async Task<IEnumerable<HistoricalEventModel>?> GetAll()
	{
		if (base.RequiresBearToken())
		{
			await AcquireBearerTokenForClient(_httpClient);
		}

		using HttpResponseMessage response = await _httpClient.GetAsync("");
		if (!response.IsSuccessStatusCode)
		{
			_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		}
		else
		{
			var historicalEventDtos = await response.Content.ReadFromJsonAsync<IEnumerable<HistoricalEventDto>>();
			if (historicalEventDtos != null)
			{
				return historicalEventDtos.Select(historicalEventDto => historicalEventDto.ToHistoricalEventModel());
			}

		}
		return default;
	}

	public async Task<PageResult<HistoricalEventViewingModel>> GetAll(PageRequest pagedRequest)
	{
		if (base.RequiresBearToken())
		{
			await AcquireBearerTokenForClient(_httpClient);
		}

		var pageRequestDto = pagedRequest.ToPageRequestDto();		
		using HttpResponseMessage response = await _httpClient.PostAsJsonAsync("PagedRequest", pageRequestDto);
		if (!response.IsSuccessStatusCode)
		{
			_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		}
		else
		{
			var resultDto = await response.Content.ReadFromJsonAsync<PageResultDto<HistoricalEventDto>>();
			if (resultDto != null)
			{
				var results = resultDto.ToPageResult<HistoricalEventDto, HistoricalEventViewingModel>();
				results.Collection = resultDto.Collection.Select(historicalEventDto => historicalEventDto.ToHistoricalEventViewingModel());
				return results;
			}
		}
		
		return new PageResult<HistoricalEventViewingModel>()
		{
			Start = 0,
			PageSize = pagedRequest.PageSize,
			ItemCount = 0,
			Collection = []
		};
	}

	public async Task<HistoricalEventModel?> Get(int id)
	{
		if (base.RequiresBearToken())
		{
			await AcquireBearerTokenForClient(_httpClient);
		}

		using HttpResponseMessage response = await _httpClient.GetAsync($"{id}");
		if (!response.IsSuccessStatusCode)
		{
			_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		}
		else
		{
			var historicalEventDto = await response.Content.ReadFromJsonAsync<HistoricalEventDto>();
			if (historicalEventDto != null)
			{
				return historicalEventDto.ToHistoricalEventModel();
			}
		}
		
		return null;
	}

	public async Task<IEnumerable<DataRecordModel>?> GetDataRecords(int id)
	{
		if (base.RequiresBearToken())
		{
			await AcquireBearerTokenForClient(_httpClient);
		}

		using HttpResponseMessage response = await _httpClient.GetAsync($"{id}/GetRecords");
		if (!response.IsSuccessStatusCode)
		{
			_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		}
		else
		{
			var dataRecordDtos = await response.Content.ReadFromJsonAsync<IEnumerable<DataRecordDto>>();
			if (dataRecordDtos != null)
			{
				return dataRecordDtos.Select(dataRecordDto => dataRecordDto.ToDataRecordModel());
			}
		}
		return default;
	}

	public async Task<bool> Exists(int id, string name)
	{
		if (base.RequiresBearToken())
		{
			await AcquireBearerTokenForClient(_httpClient);
		}

		var getHistoricalEventDto = new GetHistoricalEventDto(id, name);
		using HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"Exists", getHistoricalEventDto);
		if (response.IsSuccessStatusCode)
		{
			return await response.Content.ReadFromJsonAsync<bool>();
		}
		_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		return false;
	}

	public async Task<int> Create(HistoricalEventModel item)
	{
		if (base.RequiresBearToken())
		{
			await AcquireBearerTokenForClient(_httpClient);
		}

		var createHistoricalEventDto = item.ToCreateHistoricalEventDto();
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"", createHistoricalEventDto);
		if (response.IsSuccessStatusCode)
		{
			return await response.Content.ReadFromJsonAsync<int>();
		}
		_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		return 0;
	}

	public async Task<int> CreateDataRecord(int id, DataRecordModel item)
	{
        if (base.RequiresBearToken())
		{
			await AcquireBearerTokenForClient(_httpClient);
		}

		var createdataRecordDto = item.ToCreateRecordDto();
		using HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{id}/AddRecord", createdataRecordDto);
		if (response.IsSuccessStatusCode)
		{
            return await response.Content.ReadFromJsonAsync<int>();
		}
		_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		return 0;
	}

    public async Task<bool> DataRecordExists(int id, string shard)
    {
        if (base.RequiresBearToken())
        {
            await AcquireBearerTokenForClient(_httpClient);
        }

        using HttpResponseMessage response = await _httpClient.GetAsync($"{id}/Record?shard={shard}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<bool>();
        }
        _logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
        return true;
    }

    public async Task<bool> Update(HistoricalEventModel item, int id)
	{
		if (base.RequiresBearToken())
		{
			await AcquireBearerTokenForClient(_httpClient);
		}

		var updateHistoricalEventDto = item.ToUpdateHistoricalEventDto();
        using HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{id}", updateHistoricalEventDto);
		if (response.IsSuccessStatusCode)
		{
            return await response.Content.ReadFromJsonAsync<bool>();
        }
		_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		return false;
	}

	public async Task<bool> Delete(int id)
	{
		if (base.RequiresBearToken())
		{
			await AcquireBearerTokenForClient(_httpClient);
		}

		using HttpResponseMessage response = await _httpClient.DeleteAsync($"{id}");
		if (response.IsSuccessStatusCode)
		{
            return await response.Content.ReadFromJsonAsync<bool>();
        }
		_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		return false;
	}

	public async Task<bool> DeleteRecord(int id, int recordId)
	{
		if (base.RequiresBearToken())
		{
			await AcquireBearerTokenForClient(_httpClient);
		}

		using HttpResponseMessage response = await _httpClient.DeleteAsync($"{id}/Record/{recordId}");
		if (response.IsSuccessStatusCode)
		{
			return await response.Content.ReadFromJsonAsync<bool>();
		}
		_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		return false;
	}
}
