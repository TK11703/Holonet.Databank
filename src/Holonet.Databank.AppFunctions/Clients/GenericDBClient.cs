using Holonet.Databank.Core.Entities;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text;

namespace Holonet.Databank.AppFunctions.Clients;
public class GenericDBClient(HttpClient httpClient, ILogger<GenericDBClient> logger)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<GenericDBClient> _logger = logger;

    public async Task<bool> IsDBReady()
    {
        using HttpResponseMessage response = await _httpClient.GetAsync("DBAwake");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<bool>();
        }
        _logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
        return false;
    }    
}
