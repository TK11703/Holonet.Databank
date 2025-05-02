using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Holonet.Databank.AppFunctions.Clients;
public class SpeciesClient(HttpClient httpClient, ILogger<SpeciesClient> logger, IConfiguration configuration)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<SpeciesClient> _logger = logger;
    private readonly IConfiguration _configuration = configuration;

    public async Task<bool> UpdateDataRecord(int recordId, int speciesId, string shard, string recordText)
    {
        Guid funcIdentityGuid = Guid.Parse(_configuration.GetValue<string>("FunctionIdentityGuid")!);
        var updateRecordDto = new UpdateRecordDto(recordId, shard, recordText, null, null, null, speciesId, funcIdentityGuid);

        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{speciesId}/UpdateRecord/{recordId}", updateRecordDto);
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

    public async Task<IEnumerable<SpeciesDto>?> GetAll()
    {
        var getEntityCollectionDto = new GetEntityCollectionDto(true, true, null);
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync("GetAll", getEntityCollectionDto);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
        }
        else
        {
            return await response.Content.ReadFromJsonAsync<IEnumerable<SpeciesDto>>();            
        }
        return default;
    }

    public async Task<IEnumerable<SpeciesDto>?> GetAllUpdatedAfter(DateTime specifiedDate)
    {
        var getEntityCollectionDto = new GetEntityCollectionDto(true, true, specifiedDate.ToUniversalTime().Ticks);
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync("UpdatedSince", getEntityCollectionDto);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
        }
        else
        {
            return await response.Content.ReadFromJsonAsync<IEnumerable<SpeciesDto>>();
        }
        return default;
    }
}
