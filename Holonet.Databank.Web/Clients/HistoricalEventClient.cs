using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Models;
using Holonet.Databank.Web.Models;
using Microsoft.Identity.Web;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
namespace Holonet.Databank.Web.Clients;

public sealed class HistoricalEventClient : ClientBase
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<HistoricalEventClient> _logger;

	public HistoricalEventClient(HttpClient httpClient, ILogger<HistoricalEventClient> logger, ITokenAcquisition tokenAcquisition, IConfiguration configuration) : base(tokenAcquisition, configuration)
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

	public async Task<PageResult<HistoricalEventModel>> GetAll(PageRequest pagedRequest)
	{
		if (base.RequiresBearToken())
		{
			await AcquireBearerTokenForClient(_httpClient);
		}

		var pageRequestDto = pagedRequest.ToPageRequestDto();
		var request = new HttpRequestMessage(HttpMethod.Get, "PagedRequest")
		{
			Content = new StringContent(JsonSerializer.Serialize(pageRequestDto), Encoding.UTF8, MediaTypeNames.Application.Json)
		};
		using HttpResponseMessage response = await _httpClient.SendAsync(request);
		if (!response.IsSuccessStatusCode)
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

	public async Task<bool> CreateDataRecord(int id, DataRecordModel item)
	{
		if (base.RequiresBearToken())
		{
			await AcquireBearerTokenForClient(_httpClient);
		}

		var createdataRecordDto = item.ToCreateRecordDto();
		using HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{id}/AddRecord", createdataRecordDto);
		if (response.IsSuccessStatusCode)
		{
			return await response.Content.ReadFromJsonAsync<bool>();
		}
		_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		return false;
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
