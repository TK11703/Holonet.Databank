using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Web.Models;
using Microsoft.Identity.Web;
using System.Net.Http.Headers;

namespace Holonet.Databank.Web.Clients;

public sealed class AuthorClient
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<AuthorClient> _logger;
	private readonly ITokenAcquisition _tokenAcquisition;
	private readonly IEnumerable<string> _scopes;

	public AuthorClient(HttpClient httpClient, ILogger<AuthorClient> logger, ITokenAcquisition tokenAcquisition, IConfiguration configuration)
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
			_logger.LogError("BaseAddress of AuthorClient cannot be null.");
		}
	}

	public async Task<string> GetBearerToken()
	{
		var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(_scopes);
		if (!string.IsNullOrEmpty(accessToken))
		{
			return accessToken;
		}
		return string.Empty;
	}

	public async Task<AuthorModel?> Get(Guid azureId)
	{
		await AcquireBearerTokenForClient();

		using HttpResponseMessage response = await _httpClient.GetAsync($"{azureId}");
		if (!response.IsSuccessStatusCode)
		{
			var content = await response.Content.ReadAsStringAsync();
			_logger.LogError("Http Status:{StatusCode}\nHttp Message: {Content}", response.StatusCode, content);
		}
		else
		{
			var authorDto = await response.Content.ReadFromJsonAsync<AuthorDto>();
			if (authorDto != null)
			{
				return authorDto.ToAuthorModel();
			}
		}
		return default;
	}

	public async Task<int> Create(AuthorModel item)
	{
		await AcquireBearerTokenForClient();

		var createAuthorDto = item.ToCreateAuthorDto();
		using HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"", createAuthorDto);
		if (response.IsSuccessStatusCode)
		{
			return await response.Content.ReadFromJsonAsync<int>();
		}
		_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		return 0;
	}

	public async Task<bool> Update(AuthorModel item, int id)
	{
		await AcquireBearerTokenForClient();

		var updateAuthorDto = item.ToUpdateAuthorDto();
		using HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{id}", updateAuthorDto);
		if (response.IsSuccessStatusCode)
		{
			return await response.Content.ReadFromJsonAsync<bool>();
		}
		_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		return false;
	}	
}
