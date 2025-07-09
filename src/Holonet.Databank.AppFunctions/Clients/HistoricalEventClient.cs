using Holonet.Databank.AppFunctions.Configuration;
using Holonet.Databank.Core.Dtos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Holonet.Databank.AppFunctions.Clients;
public class HistoricalEventClient(HttpClient httpClient, ILogger<HistoricalEventClient> logger, IOptions<AppSettings> options)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<HistoricalEventClient> _logger = logger;
    private readonly AppSettings _appSettings = options.Value;

    public async Task<bool> UpdateDataRecord(int recordId, int historicalEventId, string shard, string recordText)
    {

        Guid funcIdentityGuid = Guid.Parse(_appSettings.FunctionIdentityGuid!);
        var updateRecordDto = new UpdateRecordDto(recordId, shard, recordText, null, historicalEventId, null, null, funcIdentityGuid);

        using HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{historicalEventId}/UpdateRecord/{recordId}", updateRecordDto);
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

    public async Task<IEnumerable<HistoricalEventDto>?> GetAll()
    {
        var getEntityCollectionDto = new GetEntityCollectionDto(true, true, null);
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync("GetAll", getEntityCollectionDto);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
        }
        else
        {
            return await response.Content.ReadFromJsonAsync<IEnumerable<HistoricalEventDto>>();
        }
        return default;
    }

    public async Task<IEnumerable<HistoricalEventDto>?> GetAllUpdatedAfter(DateTime specifiedDate)
    {
        var getEntityCollectionDto = new GetEntityCollectionDto(true, true, specifiedDate.ToUniversalTime().Ticks);
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync("UpdatedSince", getEntityCollectionDto);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
        }
        else
        {
            return await response.Content.ReadFromJsonAsync<IEnumerable<HistoricalEventDto>>();
        }
        return default;
    }
}
