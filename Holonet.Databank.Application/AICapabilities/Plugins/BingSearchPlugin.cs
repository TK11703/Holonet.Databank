using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;

namespace Holonet.Databank.Application.AICapabilities.Plugins;
public class BingSearchPlugin(IHttpClientFactory httpClientFactory, IConfiguration configuration)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly string _apiKey = configuration["Bing:ApiKey"] ?? throw new MissingFieldException("Bing:ApiKey");
    private readonly string _endpoint = configuration["Bing:Endpoint"] ?? throw new MissingFieldException("Bing:Endpoint");

    [KernelFunction("bing_search")]
    [Description("Executes a search on Bing. This is used when the user needs additional information. It simulates a query or search to another databank. The search result and source information is returned.")]
    [return: Description("A formatted response that contains details of the first search response. The details include the title, URL, brief result snippet, and the source of the data.")]
    public async Task<string> SearchSiteAsync(
        [Description("The address of the site that should be the focus of the Bing search.")] string site,
        [Description("The query to execute on Bing.")] string query)
    {
        using HttpClient httpClient = _httpClientFactory.CreateClient();

        string queryString = $"{query} site:{site}";
        var requestUri = $"{_endpoint}?q={Uri.EscapeDataString(queryString)}";
        httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiKey);
        HttpResponseMessage response = await httpClient.GetAsync(requestUri);

        if (response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            return ParseFirstResult(content, site);
        }
        else
        {
            return $"Error: {response.ReasonPhrase}";
        }
    }

    private static string ParseFirstResult(string jsonResponse, string source)
    {
        using (JsonDocument document = JsonDocument.Parse(jsonResponse))
        {
            var resultItems = document.RootElement
                                       .GetProperty("webPages")
                                       .GetProperty("value");

            JsonElement item = resultItems.EnumerateArray().FirstOrDefault<JsonElement>();

            string title = item.GetProperty("name").GetString() ?? string.Empty;
            string url = item.GetProperty("url").GetString() ?? string.Empty;
            string description = item.GetProperty("snippet").GetString() ?? string.Empty;

            return $"Title: {title}\nURL: {url}\nDescription: {description}\nSource: {source}";
        }
    }
}
