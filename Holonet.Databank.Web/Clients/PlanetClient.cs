using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Models;
using Holonet.Databank.Web.Models;
using Microsoft.Identity.Web;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
namespace Holonet.Databank.Web.Clients;

public sealed class PlanetClient : ClientBase
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<PlanetClient> _logger;

	public PlanetClient(HttpClient httpClient, ILogger<PlanetClient> logger, ITokenAcquisition tokenAcquisition, IConfiguration configuration) : base(tokenAcquisition, configuration)
	{
		_httpClient = httpClient;
		_logger = logger;
		PerformClientChecks();
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
			var planetDtos = await response.Content.ReadFromJsonAsync<IEnumerable<PlanetDto>>();
			if (planetDtos != null)
			{
				return planetDtos.Select(planetDto => planetDto.ToPlanetModel());
			}
		}
		return default;
	}

	public async Task<PageResult<PlanetViewingModel>> GetAll(PageRequest pagedRequest)
	{
		if (base.RequiresBearToken())
		{
			await AcquireBearerTokenForClient(_httpClient);
		}

		var pageRequestDto = pagedRequest.ToPageRequestDto();
		var request = new HttpRequestMessage(HttpMethod.Post, "PagedRequest")
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
			var resultDto = await response.Content.ReadFromJsonAsync<PageResultDto<PlanetDto>>();
			if (resultDto != null)
			{
				var results = resultDto.ToPageResult<PlanetDto, PlanetViewingModel>();
				results.Collection = resultDto.Collection.Select(planetDto => planetDto.ToPlanetViewingModel());
				return results;
			}
		}

		return new PageResult<PlanetViewingModel>()
		{
			Start = 0,
			PageSize = pagedRequest.PageSize,
			ItemCount = 0,
			Collection = []
		};
	}

	public async Task<PlanetModel?> Get(int id)
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
			var planetDto = await response.Content.ReadFromJsonAsync<PlanetDto>();
			if (planetDto != null)
			{
				return planetDto.ToPlanetModel();
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

	public async Task<bool> Exists(int id, string name)
	{
		if (base.RequiresBearToken())
		{
			await AcquireBearerTokenForClient(_httpClient);
		}

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
		if (base.RequiresBearToken())
		{
			await AcquireBearerTokenForClient(_httpClient);
		}

		var createPlanetDto = item.ToCreatePlanetDto();
		using HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"", createPlanetDto);
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

	public async Task<bool> Update(PlanetModel item, int id)
	{
		if (base.RequiresBearToken())
		{
			await AcquireBearerTokenForClient(_httpClient);
		}

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
