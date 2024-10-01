using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Models;
using Holonet.Databank.Web.Models;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
namespace Holonet.Databank.Web.Clients;

public sealed class PlanetClient
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<PlanetClient> _logger;

	public PlanetClient(HttpClient httpClient, ILogger<PlanetClient> logger)
	{
		_httpClient = httpClient;
		_logger = logger;
	}

	public async Task<IEnumerable<PlanetModel>?> GetAll()
	{
		using HttpResponseMessage response = await _httpClient.GetAsync("");
		if (!response.IsSuccessStatusCode)
		{
			_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		}
		else
		{
			var planetDtos = await response.Content.ReadFromJsonAsync<IEnumerable<PlanetDto>>();
			if (planetDtos != null)
			{
				return planetDtos.Select(planetDto => planetDto.ToPlanetModel());
			}
		}
		return default;
	}

	public async Task<PageResult<PlanetModel>> GetAll(PageRequest pagedRequest)
	{
		if (_httpClient.BaseAddress == null)
		{
			_logger.LogError("BaseAddress of PlanetClient cannot be null.");
			throw new InvalidOperationException("BaseAddress of PlanetClient cannot be null.");
		}
		var pageRequestDto = pagedRequest.ToPageRequestDto();
		var request = new HttpRequestMessage()
		{
			Method = HttpMethod.Get,
			RequestUri = new Uri(Path.Combine(_httpClient.BaseAddress.AbsoluteUri, "PagedRequest")),
			Content = new StringContent(JsonSerializer.Serialize(pageRequestDto), Encoding.UTF8, MediaTypeNames.Application.Json)
		};
		using HttpResponseMessage response = await _httpClient.SendAsync(request);
		if (!response.IsSuccessStatusCode)
		{
			_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		}
		else
		{
			var resultDto = await response.Content.ReadFromJsonAsync<PageResultDto<PlanetDto>>();
			if (resultDto != null)
			{
				var results = resultDto.ToPageResult<PlanetDto, PlanetModel>();
				results.Collection = resultDto.Collection.Select(planetDto => planetDto.ToPlanetModel());
				return results;
			}
		}

		return new PageResult<PlanetModel>()
		{
			Start = 0,
			PageSize = pagedRequest.PageSize,
			ItemCount = 0,
			Collection = []
		};
	}

	public async Task<PlanetModel?> Get(int id)
	{
		using HttpResponseMessage response = await _httpClient.GetAsync($"{id}");
		if (!response.IsSuccessStatusCode)
		{
			_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		}
		else
		{
			var planetDto = await response.Content.ReadFromJsonAsync<PlanetDto>();
			if (planetDto != null)
			{
				return planetDto.ToPlanetModel();
			}
		}
		return default;
	}

	public async Task<int> Create(PlanetModel item)
	{
		var createPlanetDto = item.ToCreatePlanetDto();
		using HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"", createPlanetDto);
		if (response.IsSuccessStatusCode)
		{
			return await response.Content.ReadFromJsonAsync<int>();
		}
		_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		return 0;
	}

	public async Task<bool> Update(PlanetModel item, int id)
	{
		var updatePlanetDto = item.ToUpdatePlanetDto();
		using HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{id}", updatePlanetDto);
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
