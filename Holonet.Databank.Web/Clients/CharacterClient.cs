using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Models;
using Holonet.Databank.Web.Models;
using Microsoft.Identity.Web;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
namespace Holonet.Databank.Web.Clients;

public sealed class CharacterClient : ClientBase
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<CharacterClient> _logger;
	public CharacterClient(HttpClient httpClient, ILogger<CharacterClient> logger, ITokenAcquisition tokenAcquisition, IConfiguration configuration) : base(tokenAcquisition, configuration)
	{
		_httpClient = httpClient;
		_logger = logger;
		PerformClientChecks();
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

	public async Task<bool> Exists(int id, string givenName, string? familyName, int? planetId)
	{
		if (base.RequiresBearToken())
		{
			await AcquireBearerTokenForClient(_httpClient);
		}

		var getCharacterDto = new GetCharacterDto(id, givenName, familyName, planetId);
		using HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"/exists", getCharacterDto);
		if (response.IsSuccessStatusCode)
		{
			return await response.Content.ReadFromJsonAsync<bool>();
		}
		_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		return false;
	}

	public async Task<CharacterModel?> Get(int id)
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
			var characterDto = await response.Content.ReadFromJsonAsync<CharacterDto>();
			if (characterDto != null)
			{
				return characterDto.ToCharacterModel();
			}
		}
		return default;
	}

	public async Task<IEnumerable<DataRecordModel>?> GetDataRecords(int id)
	{
		if (base.RequiresBearToken())
		{
			await AcquireBearerTokenForClient(_httpClient);
		}

		using HttpResponseMessage response = await _httpClient.GetAsync($"{id}/GetRecords");
		if (!response.IsSuccessStatusCode)
		{
			_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		}
		else
		{
			var dataRecordDtos = await response.Content.ReadFromJsonAsync<IEnumerable<DataRecordDto>>();
			if (dataRecordDtos != null)
			{
				return dataRecordDtos.Select(dataRecordDto => dataRecordDto.ToDataRecordModel());
			}
		}
		return default;
	}

	public async Task<int> Create(CharacterModel item)
	{
		if (base.RequiresBearToken())
		{
			await AcquireBearerTokenForClient(_httpClient);
		}

		var createCharacterDto = item.ToCreateCharacterDto();
		using HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"", createCharacterDto);
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

	public async Task<bool> Update(CharacterModel item, int id)
	{
		if (base.RequiresBearToken())
		{
			await AcquireBearerTokenForClient(_httpClient);
		}

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
