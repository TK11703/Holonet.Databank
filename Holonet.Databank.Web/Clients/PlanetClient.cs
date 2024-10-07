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

public sealed class PlanetClient
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<PlanetClient> _logger;
	private readonly ITokenAcquisition _tokenAcquisition;
	private readonly IEnumerable<string> _scopes;

	public PlanetClient(HttpClient httpClient, ILogger<PlanetClient> logger, ITokenAcquisition tokenAcquisition, IConfiguration configuration)
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
			return section.Get<IEnumerable<string>>() ?? Array.Empty<string>();
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
			_logger.LogError("BaseAddress of PlanetClient cannot be null.");
		}
	}

	public async Task<IEnumerable<PlanetModel>?> GetAll()
	{
		await AcquireBearerTokenForClient();

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
		await AcquireBearerTokenForClient();

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
		await AcquireBearerTokenForClient();

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

	public async Task<bool> Exists(int id, string name)
	{
		await AcquireBearerTokenForClient();

		var getPlanetDto = new GetPlanetDto(id, name);
		using HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"Exists", getPlanetDto);
		if (response.IsSuccessStatusCode)
		{
			return await response.Content.ReadFromJsonAsync<bool>();
		}
		_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		return false;
	}

	public async Task<int> Create(PlanetModel item)
	{
		await AcquireBearerTokenForClient();

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
		await AcquireBearerTokenForClient();

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
