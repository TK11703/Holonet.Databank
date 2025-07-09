namespace Holonet.Databank.Web.Configuration;

public class AppSettings
{
    public ApiGatewaySettings ApiGateway { get; set; }
    public ApiGatewaySettings FunctionGateway { get; set; }

    public AppSettings()
    {
        ApiGateway = new ApiGatewaySettings();
        FunctionGateway = new ApiGatewaySettings();
    }
}

public class ApiGatewaySettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKeyHeaderName { get; set; } = string.Empty;
    public string ApiKeyHeaderValue { get; set; } = string.Empty;
    public string Scopes { get; set; } = string.Empty;
    public bool RequiresBearerToken { get; set; } = false;
}