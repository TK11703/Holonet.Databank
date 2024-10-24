using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Models;
using Holonet.Databank.Web.Models;
using Microsoft.Graph.Models.ExternalConnectors;
using Microsoft.Identity.Web;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
namespace Holonet.Databank.Web.Clients;

public sealed class HistoricalEventClient
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<HistoricalEventClient> _logger;
	private readonly ITokenAcquisition _tokenAcquisition;
	private readonly IEnumerable<string> _scopes;

	public HistoricalEventClient(HttpClient httpClient, ILogger<HistoricalEventClient> logger, ITokenAcquisition tokenAcquisition, IConfiguration configuration)
	{
		_httpClient = httpClient;
		_logger = logger;
		_tokenAcquisition = tokenAcquisition;
		_scopes = GetScopesFromConfiguration(configuration);
		PerformClientChecks();
	}

	private static IEnumerable<string> GetScopesFromConfiguration(IConfiguration configuration)
	{
		var section = configuration.GetSection("DatabankApi:Scopes");
		if (section.Exists())
		{
			return section.Get<string>()?.Split(' ') ?? Array.Empty<string>();
		}
		else
		{
			return Array.Empty<string>();
		}
	}

	private async Task AcquireBearerTokenForClient()
	{
		var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(_scopes);
		if (!string.IsNullOrEmpty(accessToken))
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
		}
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
		await AcquireBearerTokenForClient();

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
		await AcquireBearerTokenForClient();

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
		await AcquireBearerTokenForClient();

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
		await AcquireBearerTokenForClient();

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
		await AcquireBearerTokenForClient();

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
		await AcquireBearerTokenForClient();

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
		await AcquireBearerTokenForClient();

		using HttpResponseMessage response = await _httpClient.DeleteAsync($"{id}");
		if (response.IsSuccessStatusCode)
		{
            return await response.Content.ReadFromJsonAsync<bool>();
        }
		_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		return false;
	}
}
