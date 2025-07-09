namespace Holonet.Databank.API.Configuration;

public class AppSettings
{
    public AiServiceSettings AzureOpenAi { get; set; }
    public AiServiceSettings AzureAiSearch { get; set; }
    public AiServiceSettings AzureAiLanguage { get; set; }

    public string GeoCodingApiKey { get; set; } = string.Empty;

    public bool ShowSwagger { get; set; } = true;


    public AppSettings()
    {
        AzureOpenAi = new AiServiceSettings();
        AzureAiSearch = new AiServiceSettings();
        AzureAiLanguage = new AiServiceSettings();
    }
}

public class AiServiceSettings
{
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Index { get; set; } = string.Empty;
    public string EmbeddingModel { get; set; } = string.Empty;
}