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

public sealed class SpeciesClient
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<SpeciesClient> _logger;
	private readonly ITokenAcquisition _tokenAcquisition;
	private readonly IEnumerable<string> _scopes;

	public SpeciesClient(HttpClient httpClient, ILogger<SpeciesClient> logger, ITokenAcquisition tokenAcquisition, IConfiguration configuration)
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
			_logger.LogError("BaseAddress of SpeciesClient cannot be null.");
		}
	}

	public async Task<IEnumerable<SpeciesModel>?> GetAll()
	{
		await AcquireBearerTokenForClient();

		using HttpResponseMessage response = await _httpClient.GetAsync("");
		if (!response.IsSuccessStatusCode)
		{
			_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		}
		else
		{
			var speciesDtos = await response.Content.ReadFromJsonAsync<IEnumerable<SpeciesDto>>();
			if (speciesDtos != null)
			{
				return speciesDtos.Select(speciesDto => speciesDto.ToSpeciesModel());
			}
		}
		return default;
	}

	public async Task<PageResult<SpeciesModel>> GetAll(PageRequest pagedRequest)
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
			var resultDto = await response.Content.ReadFromJsonAsync<PageResultDto<SpeciesDto>>();
			if (resultDto != null)
			{
				var results = resultDto.ToPageResult<SpeciesDto, SpeciesModel>();
				results.Collection = resultDto.Collection.Select(speciesDto => speciesDto.ToSpeciesModel());
				return results;
			}
		}

		return new PageResult<SpeciesModel>()
		{
			Start = 0,
			PageSize = pagedRequest.PageSize,
			ItemCount = 0,
			Collection = []
		};
	}

	public async Task<SpeciesModel?> Get(int id)
	{
		await AcquireBearerTokenForClient();

		using HttpResponseMessage response = await _httpClient.GetAsync($"{id}");
		if (!response.IsSuccessStatusCode)
		{
			_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		}
		else
		{
			var speciesDto = await response.Content.ReadFromJsonAsync<SpeciesDto>();
			if (speciesDto != null)
			{
				return speciesDto.ToSpeciesModel();
			}
		}
		return default;
	}

	public async Task<int> Create(SpeciesModel item)
	{
		await AcquireBearerTokenForClient();

		var createSpeciesDto = item.ToCreateSpeciesDto();
		using HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"", createSpeciesDto);
		if (response.IsSuccessStatusCode)
		{
			return await response.Content.ReadFromJsonAsync<int>();
		}
		_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		return 0;
	}

	public async Task<bool> Update(SpeciesModel item, int id)
	{
		await AcquireBearerTokenForClient();

		var updateSpeciesDto = item.ToUpdateSpeciesDto();
		using HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{id}", updateSpeciesDto);
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
