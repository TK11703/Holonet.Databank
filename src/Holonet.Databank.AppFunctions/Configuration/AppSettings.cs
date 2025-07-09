namespace Holonet.Databank.AppFunctions.Configuration;

public class AppSettings
{
    public ApiGatewaySettings ApiGateway { get; set; }
    public DataStorageSettings DataStorage { get; set; }
    public string FunctionIdentityGuid { get; set; } = string.Empty;

    public AppSettings()
    {
        ApiGateway = new ApiGatewaySettings();
        DataStorage = new DataStorageSettings();
    }
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