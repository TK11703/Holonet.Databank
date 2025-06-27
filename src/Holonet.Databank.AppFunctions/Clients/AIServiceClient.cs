using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Holonet.Databank.AppFunctions.Clients;
public class AIServiceClient(HttpClient httpClient, ILogger<AIServiceClient> logger)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<AIServiceClient> _logger = logger;

    public async Task<TextSummaryResult?> ExecuteTextSummarization(string input)
    {
        var executedOn = DateTime.UtcNow;
        _logger.LogInformation("Holonet.Databank.AIServiceClient ExecuteTextSummarization executed at: {ExecutionTime}", executedOn);

        try
        {
            var textSummaryRequest = new TextSummaryRequest() { Input = input, Summary = true, TargetLangCode = "en-us" };
            using HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Language/TextSummary", textSummaryRequest);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
            }
            else
            {
                return await response.Content.ReadFromJsonAsync<TextSummaryResult>();
            }
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Holonet.Databank.AIServiceClient ExecuteTextSummarization error: {ErrorMessage}", ex.Message);
            return null;
        }
    }
}
