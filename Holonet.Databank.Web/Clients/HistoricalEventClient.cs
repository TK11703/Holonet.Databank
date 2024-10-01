﻿using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Models;
using Holonet.Databank.Web.Models;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
namespace Holonet.Databank.Web.Clients;

public sealed class HistoricalEventClient
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<HistoricalEventClient> _logger;

	public HistoricalEventClient(HttpClient httpClient, ILogger<HistoricalEventClient> logger)
	{
		_httpClient = httpClient;
		_logger = logger;
	}

	public async Task<IEnumerable<HistoricalEventModel>?> GetAll()
	{
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

	public async Task<PageResult<HistoricalEventModel>> GetAll(PageRequest pagedRequest)
	{
		if (_httpClient.BaseAddress == null)
		{
			_logger.LogError("BaseAddress of HistoricalEventClient cannot be null.");
			throw new InvalidOperationException("BaseAddress of HistoricalEventClient cannot be null.");
		}
		var pageRequestDto = pagedRequest.ToPageRequestDto();
		var request = new HttpRequestMessage()
		{
			Method = HttpMethod.Get,
			RequestUri = new Uri(Path.Combine(_httpClient.BaseAddress.AbsoluteUri, "PagedRequest")),
			Content = new StringContent(JsonSerializer.Serialize(pageRequestDto), Encoding.UTF8, MediaTypeNames.Application.Json)
		};
		using HttpResponseMessage response = await _httpClient.SendAsync(request);
		if (response.IsSuccessStatusCode)
		{
			_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		}
		else
		{
			var resultDto = await response.Content.ReadFromJsonAsync<PageResultDto<HistoricalEventDto>>();
			if (resultDto != null)
			{
				var results = resultDto.ToPageResult<HistoricalEventDto, HistoricalEventModel>();
				results.Collection = resultDto.Collection.Select(historicalEventDto => historicalEventDto.ToHistoricalEventModel());
				return results;
			}
		}
		
		return new PageResult<HistoricalEventModel>()
		{
			Start = 0,
			PageSize = pagedRequest.PageSize,
			ItemCount = 0,
			Collection = []
		};
	}

	public async Task<HistoricalEventModel?> Get(int id)
	{
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

	public async Task<int> Create(HistoricalEventModel item)
	{
        var createHistoricalEventDto = item.ToCreateHistoricalEventDto();
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"", createHistoricalEventDto);
		if (response.IsSuccessStatusCode)
		{
			return await response.Content.ReadFromJsonAsync<int>();
		}
		_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		return 0;
	}

	public async Task<bool> Update(HistoricalEventModel item, int id)
	{
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
		using HttpResponseMessage response = await _httpClient.DeleteAsync($"{id}");
		if (response.IsSuccessStatusCode)
		{
            return await response.Content.ReadFromJsonAsync<bool>();
        }
		_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		return false;
	}
}