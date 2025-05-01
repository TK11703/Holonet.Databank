using HtmlAgilityPack;
using Microsoft.Extensions.Logging;


namespace Holonet.Databank.AppFunctions.HtmlHarvesting;
public class HtmlHarvester(ILogger<HtmlHarvester> logger)
{    
    private readonly ILogger _logger = logger;
    private readonly HttpClient client = new HttpClient();

    public async Task<string> HarvestHtml(string url)
    {
        DateTime executedOn = DateTime.UtcNow;
        _logger.LogInformation("Holonet.Databank.HtmlHarvesting HtmlHarvester executed at: {ExecutionTime}", executedOn);
        
        try
        {
            string pageHtml = await GetPageHtml(url);
            if (!string.IsNullOrEmpty(pageHtml))
            {
                return GetPageBody(pageHtml);
            }
            return string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Holonet.Databank.HtmlHarvesting HtmlHarvester error: {ErrorMessage}", ex.Message);
            return "An internal server error occurred.";
        }
    }

    private async Task<string> GetPageHtml(string url)
    {
        var result = string.Empty;

        try
        {
            result = await client.GetStringAsync(url);
        }
        catch (HttpRequestException ex)
        {
            LogMessage($"Unable to GET requested URL ({url}). Error: {ex.Message}", LogLevel.Error);
        }

        return result;
    }

    private static string GetPageBody(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var body = doc.DocumentNode.SelectSingleNode("//body");
        return body?.InnerHtml ?? string.Empty;
    }

    private void LogMessage(string message, LogLevel logLevel = LogLevel.Information)
    {
        var msg = $"Logger, UTC:{DateTime.UtcNow} => {message}";
        _logger.Log(logLevel, msg);
        System.Diagnostics.Debug.WriteLine(msg);
    }
}
