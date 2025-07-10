namespace Holonet.Databank.AppFunctions.Configuration;

public class AppSettings
{
    public ApiGatewaySettings ApiGateway { get; set; }
    public DataStorageSettings DataStorage { get; set; }
    public CustomHttpHeaders HarvestingHttpReqHeaders { get; set; }
    public string FunctionIdentityGuid { get; set; } = string.Empty;

    public AppSettings()
    {
        ApiGateway = new ApiGatewaySettings();
        DataStorage = new DataStorageSettings();
        HarvestingHttpReqHeaders = new CustomHttpHeaders();
    }
}

public class CustomHttpHeaders
{
    public string? UserAgent { get; set; } = string.Empty;
    public string? Referrer { get; set; } = string.Empty;
    public string? Accept { get; set; } = string.Empty;
    public string? AcceptLanguage { get; set; } = string.Empty;
    public string? AcceptEncoding { get; set; } = string.Empty;
    public string? UpgradeInsecureRequests { get; set; } = string.Empty;
    public string? CacheControl { get; set; } = string.Empty;
    public string? SecCHUA { get; set; } = string.Empty;
    public string? SecCHUAPlatform { get; set; } = string.Empty;
    public string? SecCHUAMobile { get; set; } = string.Empty;
    public string? SecFetchDest { get; set; } = string.Empty;
    public string? SecFetchMode { get; set; } = string.Empty;
    public string? SecFetchSite { get; set; } = string.Empty;
    public string? SecFetchUser { get; set; } = string.Empty;
}

public class DataStorageSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
}
public class ApiGatewaySettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKeyHeaderName { get; set; } = string.Empty;
    public string ApiKeyHeaderValue { get; set; } = string.Empty;
    public string Scopes { get; set; } = string.Empty;
    public bool RequiresBearerToken { get; set; } = false;
}