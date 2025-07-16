using Holonet.Databank.Core.Dtos;
using Holonet.Databank.AppFunctions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Holonet.Databank.AppFunctions.Clients;
public class CharacterClient(HttpClient httpClient, ILogger<CharacterClient> logger, IOptions<AppSettings> options)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<CharacterClient> _logger = logger;
    private readonly AppSettings _appSettings = options.Value;

    public async Task<bool> UpdateDataRecordForProcessing(int recordId, int characterId, string shard)
    {
        Guid funcIdentityGuid = Guid.Parse(_appSettings.FunctionIdentityGuid!);
        var updateRecordDto = new UpdateRecordDto(recordId, shard, string.Empty, false, true, false, null, characterId, null, null, null, funcIdentityGuid);

        using HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{characterId}/UpdateRecord/{recordId}", updateRecordDto);
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

    public async Task<bool> UpdateDataRecordForProcessingError(int recordId, int characterId, string shard, string errorMessage)
    {
        Guid funcIdentityGuid = Guid.Parse(_appSettings.FunctionIdentityGuid!);
        var updateRecordDto = new UpdateRecordDto(recordId, shard, string.Empty, false, false, false, errorMessage, characterId, null, null, null, funcIdentityGuid);

        using HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{characterId}/UpdateRecord/{recordId}", updateRecordDto);
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

    public async Task<bool> UpdateDataRecord(int recordId, int characterId, string shard, string recordText)
    {
        Guid funcIdentityGuid = Guid.Parse(_appSettings.FunctionIdentityGuid!);
        var updateRecordDto = new UpdateRecordDto(recordId, shard, recordText, false, false, true, null, characterId, null, null, null, funcIdentityGuid);

        using HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{characterId}/UpdateRecord/{recordId}", updateRecordDto);
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

    public async Task<IEnumerable<CharacterDto>?> GetAll()
    {
        var getEntityCollectionDto = new GetEntityCollectionDto(true, true, null);
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync("GetAll", getEntityCollectionDto);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
        }
        else
        {
            return await response.Content.ReadFromJsonAsync<IEnumerable<CharacterDto>>();
        }
        return default;
    }

    public async Task<IEnumerable<CharacterDto>?> GetAllUpdatedAfter(DateTime specifiedDate)
    {
        var getEntityCollectionDto = new GetEntityCollectionDto(true, true, specifiedDate.ToUniversalTime().Ticks);
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync("UpdatedSince", getEntityCollectionDto);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
        }
        else
        {
            return await response.Content.ReadFromJsonAsync<IEnumerable<CharacterDto>>();
        }
        return default;
    }

}
