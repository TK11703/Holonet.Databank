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

    public async Task<string> HarvestHtml(string url)
    {
        DateTime executedOn = DateTime.UtcNow;
        _logger.LogInformation("Holonet.Databank.HtmlHarvesting HtmlHarvester executed at: {ExecutionTime}", executedOn);

        try
        {
            string pageHtml = await GetPageHtml(url);
            if (!string.IsNullOrEmpty(pageHtml))
            {
                return GetSiteContent(url, pageHtml);
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


            result = await client.GetStringAsync(url);
        }
        catch (HttpRequestException ex)
        {
            LogMessage($"Unable to GET requested URL ({url}). Error: {ex.Message}", LogLevel.Error);
        }

        return result;
    }

    private static string GetSiteContent(string url, string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var regex = new System.Text.RegularExpressions.Regex(@"^https:\/\/starwars\.fandom\.com\/wiki\/.*$");
        if (regex.IsMatch(url))
        {
            var textContentparagraphs = doc.DocumentNode.SelectSingleNode("//div[@id='content']")?.SelectSingleNode("//div[@id='mw-content-text']")?.SelectNodes("//p[normalize-space(.) != '']");
            if (textContentparagraphs == null)
            {
                return string.Empty;
            }
            return string.Join(Environment.NewLine, textContentparagraphs.Select(p => CleanText(p.InnerText.Trim())));
        }

        regex = new System.Text.RegularExpressions.Regex(@"^https:\/\/www\.starwars\.com\/databank\/.*$");
        if (regex.IsMatch(url))
        {
            var overviewContent = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'content-info')]")?.SelectNodes("//p[contains(@class, 'desc')]");
            var historicalContent = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'rich-text-output')]")?.SelectNodes("//p");
            if (overviewContent != null && historicalContent != null)
            {
                return string.Join(Environment.NewLine, overviewContent.Concat(historicalContent).Select(p => CleanText(p.InnerText.Trim())));
            }
            else if (overviewContent != null && historicalContent == null)
            {
                return string.Join(Environment.NewLine, overviewContent.Select(p => CleanText(p.InnerText.Trim())));
            }
            else if (overviewContent == null && historicalContent != null)
            {
                return string.Join(Environment.NewLine, historicalContent.Select(p => CleanText(p.InnerText.Trim())));
            }
            else
            {
                return string.Empty;
            }
        }
        return string.Empty;
    }

    private static string CleanText(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }
        string cleaned = Regex.Replace(input, @"(?:\[|\&#91;)\d+(?:\]|\&#93;)", "");
        return cleaned;
    }

    private void LogMessage(string message, LogLevel logLevel = LogLevel.Information)
    {
        var msg = $"Logger, UTC:{DateTime.UtcNow} => {message}";
        _logger.Log(logLevel, msg);
        System.Diagnostics.Debug.WriteLine(msg);
    }
}
