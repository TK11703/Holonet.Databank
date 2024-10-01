using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Models;
using Holonet.Databank.Web.Models;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
namespace Holonet.Databank.Web.Clients;

public sealed class CharacterClient
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<CharacterClient> _logger;

	public CharacterClient(HttpClient httpClient, ILogger<CharacterClient> logger)
	{
		_httpClient = httpClient;
		_logger = logger;
	}

	public async Task<IEnumerable<CharacterModel>?> GetAll()
	{
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
		if (_httpClient.BaseAddress == null)
		{
			_logger.LogError("BaseAddress of CharacterClient cannot be null.");
			throw new InvalidOperationException("BaseAddress of CharacterClient cannot be null.");
		}
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
		using HttpResponseMessage response = await _httpClient.DeleteAsync($"{id}");
		if (response.IsSuccessStatusCode)
		{
			return await response.Content.ReadFromJsonAsync<bool>();
		}
		_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		return false;
	}
}
