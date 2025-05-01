using Holonet.Databank.Core.Dtos;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Holonet.Databank.AppFunctions.Clients;
public class PlanetClient(HttpClient httpClient, ILogger<PlanetClient> logger)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<PlanetClient> _logger = logger;

    public async Task<bool> UpdateDataRecord(int recordId, int planetId, string shard, string recordText)
    {
        var updateRecordDto = new UpdateRecordDto(recordId, shard, recordText, null, null, planetId, null, Guid.Empty);

        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{planetId}/UpdateRecord/{recordId}", updateRecordDto);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
        }
        else
        {
            return await response.Content.ReadFromJsonAsync<bool>();
        }
        return default;
    }

    public async Task<IEnumerable<PlanetDto>?> GetAll()
    {
        var getEntityCollectionDto = new GetEntityCollectionDto(true, true, null);
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync("GetAll", getEntityCollectionDto);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
        }
        else
        {
            return await response.Content.ReadFromJsonAsync<IEnumerable<PlanetDto>>();
        }
        return default;
    }

    public async Task<IEnumerable<PlanetDto>?> GetAllUpdatedAfter(DateTime specifiedDate)
    {
        var getEntityCollectionDto = new GetEntityCollectionDto(true, true, specifiedDate.ToUniversalTime().Ticks);
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync("UpdatedSince", getEntityCollectionDto);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
        }
        else
        {
            return await response.Content.ReadFromJsonAsync<IEnumerable<PlanetDto>>();
        }
        return default;
    }
}
