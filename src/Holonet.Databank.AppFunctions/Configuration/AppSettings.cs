using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.AppFunctions.Configuration;

public class AppSettings
{
    [Required]
    public ApiGatewaySettings ApiGateway { get; set; }

    [Required]
    public DataStorageSettings DataStorage { get; set; }

    [Required]
    public CustomHttpHeaders HarvestingHttpReqHeaders { get; set; }
    
    [Required]
    public StorageQueueSettings StorageQueue { get; set; }

    [Required]
    public string FunctionIdentityGuid { get; set; } = string.Empty;

    [Required]
    public bool UseSqlTrigger { get; set; } = false;

    [Required]
    public bool UseQueueTrigger { get; set; } = false;

    public AppSettings()
    {
        ApiGateway = new ApiGatewaySettings();
        DataStorage = new DataStorageSettings();
        HarvestingHttpReqHeaders = new CustomHttpHeaders();
        StorageQueue = new StorageQueueSettings();
    }
}

public class CustomHttpHeaders
{
    [Required(ErrorMessage = "UserAgent header is required")]
    public string? UserAgent { get; set; } = string.Empty;

    [Required(ErrorMessage = "Referrer header is required")]
    public string? Referrer { get; set; } = string.Empty;

    [Required(ErrorMessage = "Accept header is required")]
    public string? Accept { get; set; } = string.Empty;

    [Required(ErrorMessage = "AcceptLanguage header is required")]
    public string? AcceptLanguage { get; set; } = string.Empty;

    [Required(ErrorMessage = "AcceptEncoding header is required")]
    public string? AcceptEncoding { get; set; } = string.Empty;

    [Required(ErrorMessage = "UpgradeInsecureRequests header is required")]
    public string? UpgradeInsecureRequests { get; set; } = string.Empty;

    [Required(ErrorMessage = "CacheControl header is required")]
    public string? CacheControl { get; set; } = string.Empty;

    [Required(ErrorMessage = "Sec-CH-UA header is required")]
    public string? SecCHUA { get; set; } = string.Empty;

    [Required(ErrorMessage = "Sec-CH-UA-Platform header is required")]
    public string? SecCHUAPlatform { get; set; } = string.Empty;

    [Required(ErrorMessage = "Sec-CH-UA-Mobile header is required")]
    public string? SecCHUAMobile { get; set; } = string.Empty;

    [Required(ErrorMessage = "Sec-Fetch-Dest header is required")]
    public string? SecFetchDest { get; set; } = string.Empty;

    [Required(ErrorMessage = "Sec-Fetch-Mode header is required")]
    public string? SecFetchMode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Sec-Fetch-Site header is required")]
    public string? SecFetchSite { get; set; } = string.Empty;

    [Required(ErrorMessage = "Sec-Fetch-User header is required")]
    public string? SecFetchUser { get; set; } = string.Empty;
}

public class DataStorageSettings
{
    [Required(ErrorMessage = "DataStorage ConnectionString is required", AllowEmptyStrings = false)]
    [MinLength(1, ErrorMessage = "DataStorage ConnectionString cannot be empty")]
    public string ConnectionString { get; set; } = string.Empty;

    [Required(ErrorMessage = "DataStorage ContainerName is required")]
    public string ContainerName { get; set; } = string.Empty;
}
public class ApiGatewaySettings
{
    [Required(ErrorMessage = "ApiGatewaySettings BaseUrl is required")]
    public string BaseUrl { get; set; } = string.Empty;

    [Required(ErrorMessage = "ApiGatewaySettings ApiKeyHeaderName is required")]
    public string ApiKeyHeaderName { get; set; } = string.Empty;

    [Required(ErrorMessage = "ApiGatewaySettings ApiKeyHeaderValue is required")]
    public string ApiKeyHeaderValue { get; set; } = string.Empty;

    [Required(ErrorMessage = "ApiGatewaySettings Scopes is required")]
    public string Scopes { get; set; } = string.Empty;

    public bool RequiresBearerToken { get; set; } = false;
}

public class StorageQueueSettings
{
    public bool UseDevStorage { get; set; } = false;
    public bool UseSAS { get; set; } = false;
    public bool UseConnectionString { get; set; } = false;
    public string BaseUrl { get; set; } = string.Empty;
    public string QueueName { get; set; } = string.Empty;
    public string SASUrl { get; set; } = string.Empty;
    public string DefaultConnection { get; set; } = string.Empty;
}