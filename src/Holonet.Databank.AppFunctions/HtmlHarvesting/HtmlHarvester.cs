using Holonet.Databank.AppFunctions.Configuration;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.RegularExpressions;

namespace Holonet.Databank.AppFunctions.HtmlHarvesting;
public class HtmlHarvester
{
    private readonly ILogger _logger;
    private readonly AppSettings _settings;
    private readonly HttpClient client;

    public HtmlHarvester(ILogger<HtmlHarvester> logger, AppSettings settings)
    {
        _logger = logger;
        _settings = settings;
        client = new HttpClient(GetCustomizedHandler());
    }

    private HttpClientHandler GetCustomizedHandler()
    {
        var handler = new HttpClientHandler
        {
            AllowAutoRedirect = true,
            UseCookies = true,
            CookieContainer = new System.Net.CookieContainer(),
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
        return handler;
    }

    public async Task<IEnumerable<string>> HarvestHtml(string url)
    {
        DateTime executedOn = DateTime.UtcNow;
        _logger.LogInformation("Holonet.Databank.HtmlHarvesting HtmlHarvester executed at: {ExecutionTime}", executedOn);
        if (string.IsNullOrWhiteSpace(url))
        {
            _logger.LogWarning("Holonet.Databank.HtmlHarvesting HtmlHarvester received an empty URL.");
            return [];
        }

        try
        {
            string pageHtml = await GetPageHtml(url);
            if (string.IsNullOrWhiteSpace(pageHtml))
            {
                _logger.LogWarning("Holonet.Databank.HtmlHarvesting HtmlHarvester received a URL, but the provided URL did not yield any page content to process.");
            }
            else
            {
                return GetSiteContentChunks(url, pageHtml);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Holonet.Databank.HtmlHarvesting HtmlHarvester error: {ErrorMessage}", ex.Message);
        }
        return [];
    }

    private async Task<string> GetPageHtml(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            _logger.LogWarning("Holonet.Databank.HtmlHarvesting HtmlHarvester->GetPageHtml received an empty URL.");
            return string.Empty;
        }

        try
        {
            SetCustomHttpRequestHeaders();
            return await client.GetStringAsync(url);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Unable to GET requested URL ({Url}). Error: {ErrorMessage}", url, ex.Message);
        }

        return string.Empty;
    }

    private void SetCustomHttpRequestHeaders()
    {
        if (!string.IsNullOrWhiteSpace(_settings.HarvestingHttpReqHeaders.UserAgent))
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd(_settings.HarvestingHttpReqHeaders.UserAgent);
        }
        //Add additional headers which might be required.
        if (!string.IsNullOrWhiteSpace(_settings.HarvestingHttpReqHeaders.Referrer))
        {
            client.DefaultRequestHeaders.Referrer = new Uri(_settings.HarvestingHttpReqHeaders.Referrer);
        }
        if (!string.IsNullOrWhiteSpace(_settings.HarvestingHttpReqHeaders.Accept))
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.ParseAdd(_settings.HarvestingHttpReqHeaders.Accept);
        }
        if (!string.IsNullOrWhiteSpace(_settings.HarvestingHttpReqHeaders.AcceptLanguage))
        {
            client.DefaultRequestHeaders.AcceptLanguage.Clear();
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(_settings.HarvestingHttpReqHeaders.AcceptLanguage);
        }
        if (!string.IsNullOrWhiteSpace(_settings.HarvestingHttpReqHeaders.CacheControl))
        {
            client.DefaultRequestHeaders.Add("Cache-Control", _settings.HarvestingHttpReqHeaders.CacheControl);
        }
        if (!string.IsNullOrWhiteSpace(_settings.HarvestingHttpReqHeaders.AcceptLanguage))
        {
            client.DefaultRequestHeaders.AcceptEncoding.Clear();
            client.DefaultRequestHeaders.AcceptEncoding.ParseAdd(_settings.HarvestingHttpReqHeaders.AcceptEncoding);
        }                
        client.DefaultRequestHeaders.ConnectionClose = false;
        if (!string.IsNullOrWhiteSpace(_settings.HarvestingHttpReqHeaders.UpgradeInsecureRequests))
        {
            client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", _settings.HarvestingHttpReqHeaders.UpgradeInsecureRequests);
        }
        if(!string.IsNullOrWhiteSpace(_settings.HarvestingHttpReqHeaders.SecCHUA))
        {
             client.DefaultRequestHeaders.Add("Sec-CH-UA", _settings.HarvestingHttpReqHeaders.SecCHUA);
        }
        if (!string.IsNullOrWhiteSpace(_settings.HarvestingHttpReqHeaders.SecCHUAPlatform))
        {
            client.DefaultRequestHeaders.Add("Sec-CH-UA-Platform", _settings.HarvestingHttpReqHeaders.SecCHUAPlatform);
        }
        if (!string.IsNullOrWhiteSpace(_settings.HarvestingHttpReqHeaders.SecCHUAMobile))
        {
            client.DefaultRequestHeaders.Add("Sec-CH-UA-Mobile", _settings.HarvestingHttpReqHeaders.SecCHUAMobile);
        }
        if (!string.IsNullOrWhiteSpace(_settings.HarvestingHttpReqHeaders.SecFetchDest))
        {
            client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", _settings.HarvestingHttpReqHeaders.SecFetchDest);
        }
        if (!string.IsNullOrWhiteSpace(_settings.HarvestingHttpReqHeaders.SecFetchMode))
        {
            client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", _settings.HarvestingHttpReqHeaders.SecFetchMode);
        }
        if (!string.IsNullOrWhiteSpace(_settings.HarvestingHttpReqHeaders.SecFetchSite))
        {
            client.DefaultRequestHeaders.Add("Sec-Fetch-Site", _settings.HarvestingHttpReqHeaders.SecFetchSite);
        }
        if (!string.IsNullOrWhiteSpace(_settings.HarvestingHttpReqHeaders.SecFetchUser))
        {
            client.DefaultRequestHeaders.Add("Sec-Fetch-User", _settings.HarvestingHttpReqHeaders.SecFetchUser);
        }
    }

    private IEnumerable<string> GetSiteContentChunks(string url, string html)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            _logger.LogWarning("Holonet.Databank.HtmlHarvesting HtmlHarvester->GetSiteContentChunks received an empty URL.");
            return [];
        }
        if (string.IsNullOrWhiteSpace(html))
        {
            _logger.LogWarning("Holonet.Databank.HtmlHarvesting HtmlHarvester->GetSiteContentChunks received an empty html parameter.");
            return [];
        }
        var regex = new Regex(@"^https:\/\/starwars\.fandom\.com\/wiki\/.*$");
        if (regex.IsMatch(url))
        {
            return GetSite1Content(html);
        }

        regex = new Regex(@"^https:\/\/www\.starwars\.com\/databank\/.*$");
        if (regex.IsMatch(url))
        {
            return GetSite2Content(html);
        }
        return [];
    }

    private IEnumerable<string> GetSite1Content(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            _logger.LogWarning("Holonet.Databank.HtmlHarvesting HtmlHarvester->GetSite1Content received an empty html parameter.");
            return [];
        }
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var contentDiv = doc.DocumentNode.SelectSingleNode("//div[@id='content']")?.SelectSingleNode(".//div[@id='mw-content-text']")?.SelectSingleNode("//div[contains(@class, 'mw-content-ltr')]");
        if (contentDiv == null)
        {
            _logger.LogWarning("Holonet.Databank.HtmlHarvesting HtmlHarvester->GetSite1Content could not find the content div in the HTML.");
            return [];
        }
        //only interested in the overview content for the URL
        var paragraphs = new List<HtmlNode>();
        foreach (var node in contentDiv.ChildNodes)
        {
            if (node.Name == "div" && node.GetAttributeValue("id", "") == "toc")
                break;

            if (node.Name == "p" && !string.IsNullOrWhiteSpace(node.InnerText))
                paragraphs.Add(node);
        }
        var first20 = paragraphs?.Take(20).ToList();
        if (first20 == null)
        {
            return [];
        }
        return first20.Select(p => CleanText(p.InnerText.Trim()));
    }

    private IEnumerable<string> GetSite2Content(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            _logger.LogWarning("Holonet.Databank.HtmlHarvesting HtmlHarvester->GetSite2Content received an empty html parameter.");
            return [];
        }
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var overviewContent = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'content-info')]")?.SelectNodes("//p[contains(@class, 'desc')]");
        var first20Overview = overviewContent?.Take(20).ToList();
        var historicalContent = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'rich-text-output')]")?.SelectNodes("./p");
        var first20Historical = historicalContent?.Take(20).ToList();
        if (first20Overview != null && first20Historical != null)
        {
            return first20Overview.Concat(first20Historical).Select(p => CleanText(p.InnerText.Trim()));
        }
        else if (first20Overview != null && first20Historical == null)
        {
            return first20Overview.Select(p => CleanText(p.InnerText.Trim()));
        }
        else if (first20Overview == null && first20Historical != null)
        {
            return first20Historical.Select(p => CleanText(p.InnerText.Trim()));
        }
        else
        {
            return [];
        }
    }

    private static string CleanText(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }
        return Regex.Replace(input, @"(?:\[|\&#91;)\d+(?:\]|\&#93;)", "");
    }
}
