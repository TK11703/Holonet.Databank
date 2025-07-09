using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Web.Models;
using Holonet.Databank.Web.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace Holonet.Databank.Web.Clients;

public sealed class AuthorClient : ClientBase
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<AuthorClient> _logger;

	public AuthorClient(HttpClient httpClient, ILogger<AuthorClient> logger, ITokenAcquisition tokenAcquisition, IOptions<AppSettings> options) : base(tokenAcquisition, options)
	{
		_httpClient = httpClient;
		_logger = logger;
		PerformClientChecks();
	}

	private void PerformClientChecks()
	{
		if (_httpClient.BaseAddress == null)
		{
			_logger.LogError("BaseAddress of AuthorClient cannot be null.");
		}
	}	

	public async Task<AuthorModel?> Get(Guid azureId)
	{
		if(base.RequiresBearToken())
		{
			await AcquireBearerTokenForClient(_httpClient);
		}

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
		if (base.RequiresBearToken())
		{
			await AcquireBearerTokenForClient(_httpClient);
		}

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
		if (base.RequiresBearToken())
		{
			await AcquireBearerTokenForClient(_httpClient);
		}

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
