using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Xml.Linq;


namespace Holonet.Databank.AppFunctions.HtmlHarvesting;
public class HtmlHarvester
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient client;

    public HtmlHarvester(ILogger<HtmlHarvester> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
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
            client.DefaultRequestHeaders.UserAgent.ParseAdd(_configuration.GetValue<string>("HttpClientHeaders:UserAgent")!);
            //Add additional headers which might be required.
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
            client.DefaultRequestHeaders.Referrer = new Uri("https://www.google.com"); // Optional but helps
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.5");
            client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate");
            client.DefaultRequestHeaders.ConnectionClose = false;
            client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
            client.DefaultRequestHeaders.Add("Sec-CH-UA", "\"Chromium\";v=\"122\", \"Not A;Brand\";v=\"99\", \"Google Chrome\";v=\"122\"");
            client.DefaultRequestHeaders.Add("Sec-CH-UA-Mobile", "?0");
            client.DefaultRequestHeaders.Add("Sec-CH-UA-Platform", "\"Windows\"");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "\"document\"");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "\"navigate\"");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "\"none\"");
            client.DefaultRequestHeaders.Add("Sec-Fetch-User", "\"?1\"");
            client.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");

            return await client.GetStringAsync(url);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Unable to GET requested URL ({Url}). Error: {ErrorMessage}", url, ex.Message);
        }

        return string.Empty;
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
