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

public sealed class CharacterClient
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<CharacterClient> _logger;
	private readonly ITokenAcquisition _tokenAcquisition;
	private readonly IEnumerable<string> _scopes;

	public CharacterClient(HttpClient httpClient, ILogger<CharacterClient> logger, ITokenAcquisition tokenAcquisition, IConfiguration configuration)
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
			_logger.LogError("BaseAddress of CharacterClient cannot be null.");
		}
	}

	public async Task<IEnumerable<CharacterModel>?> GetAll()
	{
		await AcquireBearerTokenForClient();

		using HttpResponseMessage response = await _httpClient.GetAsync("");
		if (!response.IsSuccessStatusCode)
		{
			_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		}
		else
		{
			var characterDtos = await response.Content.ReadFromJsonAsync<IEnumerable<CharacterDto>>();
			if (characterDtos != null)
			{
				return characterDtos.Select(characterDto => characterDto.ToCharacterModel());
			}
		}
		return default;
	}

	public async Task<PageResult<CharacterModel>> GetAll(PageRequest pagedRequest)
	{
		await AcquireBearerTokenForClient();
		
		var pageRequestDto = pagedRequest.ToPageRequestDto();
		var request = new HttpRequestMessage()
		{
			Method = HttpMethod.Get,
			RequestUri = new Uri(Path.Combine(path1: _httpClient.BaseAddress.AbsoluteUri, path2: "PagedRequest")),
			Content = new StringContent(JsonSerializer.Serialize(pageRequestDto), Encoding.UTF8, MediaTypeNames.Application.Json)
		};
		using HttpResponseMessage response = await _httpClient.SendAsync(request);
		if (!response.IsSuccessStatusCode)
		{
			_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		}
		else
		{
			var resultDto = await response.Content.ReadFromJsonAsync<PageResultDto<CharacterDto>>();
			if (resultDto != null)
			{
				var results = resultDto.ToPageResult<CharacterDto, CharacterModel>();
				results.Collection = resultDto.Collection.Select(characterDto => characterDto.ToCharacterModel());
				return results;
			}
		}
		return new PageResult<CharacterModel>()
		{
			Start = 0,
			PageSize = pagedRequest.PageSize,
			ItemCount = 0,
			Collection = []
		};
	}

	public async Task<CharacterModel?> Get(int id)
	{
		await AcquireBearerTokenForClient();

		using HttpResponseMessage response = await _httpClient.GetAsync($"{id}");
		if (!response.IsSuccessStatusCode)
		{
			_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		}
		else
		{
			var characterDto = await response.Content.ReadFromJsonAsync<CharacterDto>();
			if (characterDto != null)
			{
				return characterDto.ToCharacterModel();
			}
		}
		return default;
	}

	public async Task<int> Create(CharacterModel item)
	{
		await AcquireBearerTokenForClient();

		var createCharacterDto = item.ToCreateCharacterDto();
		using HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"", createCharacterDto);
		if (response.IsSuccessStatusCode)
		{
			return await response.Content.ReadFromJsonAsync<int>();
		}
		_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		return 0;
	}

	public async Task<bool> Update(CharacterModel item, int id)
	{
		await AcquireBearerTokenForClient();

		var updateCharacterDto = item.ToUpdateCharacterDto();
		using HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{id}", updateCharacterDto);
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
