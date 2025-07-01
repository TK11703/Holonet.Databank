using Holonet.Databank.Core.Entities;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text;

namespace Holonet.Databank.AppFunctions.Clients;
public class AIServiceClient(HttpClient httpClient, ILogger<AIServiceClient> logger)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<AIServiceClient> _logger = logger;

    public async Task<string> ExecuteTextSummarization(IEnumerable<string> input)
    {
        var executedOn = DateTime.UtcNow;
        _logger.LogInformation("Holonet.Databank.AIServiceClient ExecuteTextSummarization executed at: {ExecutionTime}", executedOn);

        var validInputs = input.Where(htmlInput => !string.IsNullOrWhiteSpace(htmlInput));
        StringBuilder sb = new();
        _logger.LogInformation("Holonet.Databank.AIServiceClient ExecuteTextSummarization will attempt to summarize {Count} html chunk(s).", validInputs.Count());
        for (int i = 0; i < validInputs.Count(); i++)
        {
            string htmlInput = validInputs.ElementAt(i);
            _logger.LogInformation("Holonet.Databank.AIServiceClient ExecuteTextSummarization processing chunk {ChunkIndex} of {TotalChunks}.", i + 1, validInputs.Count());
            TextSummaryResult? result = await ExecuteTextSummaryAsync(htmlInput);
            if (result != null && !string.IsNullOrWhiteSpace(result.ResultText))
            {
                sb.AppendLine(result.ResultText);
            }
            else
            {
                _logger.LogWarning("Holonet.Databank.AIServiceClient ExecuteTextSummarization received an empty summarization at chunk {ChunkIndex}.", i + 1);
            }
        }
        return sb.ToString().TrimEnd();
    }

    private async Task<TextSummaryResult?> ExecuteTextSummaryAsync(string input)
    {
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
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Holonet.Databank.AIServiceClient ExecuteTextSummarization error: {ErrorMessage}", ex.Message);            
        }
        return null;
    }
}
