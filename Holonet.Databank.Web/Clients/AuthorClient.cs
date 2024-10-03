using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Models;
using Holonet.Databank.Web.Models;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
namespace Holonet.Databank.Web.Clients;

public sealed class AuthorClient
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<AuthorClient> _logger;

	public AuthorClient(HttpClient httpClient, ILogger<AuthorClient> logger)
	{
		_httpClient = httpClient;
		_logger = logger;
	}

	public async Task<AuthorModel?> Get(int id)
	{
		using HttpResponseMessage response = await _httpClient.GetAsync($"{id}");
		if (!response.IsSuccessStatusCode)
		{
			_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
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
