using Holonet.Databank.Web.Clients;
using Holonet.Databank.Web.Configuration;

namespace Holonet.Databank.Web.Extensions;

internal static class ClientConfigs
{
    /// <summary>
    /// Helps configure these TYPED clients (which are transient) to be used in potential singleton services and not be affected by DNS refresh issues and port exhaustion. Otherwise 
    /// NAMED clients could be used instead with the HttpClientFactory usage.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    public static void ConfigureClients(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<GraphApiClient>();
        var apiSettings = config.TryGetSection<ApiGatewaySettings>("AppSettings:ApiGateway") ?? throw new InvalidOperationException("Missing AppSettings:ApiGateway in configuration.");
        if (!string.IsNullOrWhiteSpace(apiSettings.BaseUrl))
        {
            services.AddHttpClient<AuthorClient>((sp, client) =>
            {
                client.BaseAddress = new Uri(new Uri(apiSettings.BaseUrl), "Authors/");
                if(!string.IsNullOrEmpty(apiSettings.ApiKeyHeaderName) && !string.IsNullOrEmpty(apiSettings.ApiKeyHeaderValue))
                {
                    client.DefaultRequestHeaders.Add(apiSettings.ApiKeyHeaderName, apiSettings.ApiKeyHeaderValue);
                }
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(5)
                };
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);

            services.AddHttpClient<CharacterClient>(client =>
            {
                client.BaseAddress = new Uri(new Uri(apiSettings.BaseUrl), "Characters/");
                if (!string.IsNullOrEmpty(apiSettings.ApiKeyHeaderName) && !string.IsNullOrEmpty(apiSettings.ApiKeyHeaderValue))
                {
                    client.DefaultRequestHeaders.Add(apiSettings.ApiKeyHeaderName, apiSettings.ApiKeyHeaderValue);
                }
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(5)
                };
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);

            services.AddHttpClient<PlanetClient>(client =>
            {
                client.BaseAddress = new Uri(new Uri(apiSettings.BaseUrl), "Planets/");
                if (!string.IsNullOrEmpty(apiSettings.ApiKeyHeaderName) && !string.IsNullOrEmpty(apiSettings.ApiKeyHeaderValue))
                {
                    client.DefaultRequestHeaders.Add(apiSettings.ApiKeyHeaderName, apiSettings.ApiKeyHeaderValue);
                }
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(5)
                };
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);

            services.AddHttpClient<SpeciesClient>(client =>
            {
                client.BaseAddress = new Uri(new Uri(apiSettings.BaseUrl), "Species/");
                if (!string.IsNullOrEmpty(apiSettings.ApiKeyHeaderName) && !string.IsNullOrEmpty(apiSettings.ApiKeyHeaderValue))
                {
                    client.DefaultRequestHeaders.Add(apiSettings.ApiKeyHeaderName, apiSettings.ApiKeyHeaderValue);
                }
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(5)
                };
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);

            services.AddHttpClient<HistoricalEventClient>(client =>
            {
                client.BaseAddress = new Uri(new Uri(apiSettings.BaseUrl), "HistoricalEvents/");
                if (!string.IsNullOrEmpty(apiSettings.ApiKeyHeaderName) && !string.IsNullOrEmpty(apiSettings.ApiKeyHeaderValue))
                {
                    client.DefaultRequestHeaders.Add(apiSettings.ApiKeyHeaderName, apiSettings.ApiKeyHeaderValue);
                }
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(5)
                };
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);

            services.AddHttpClient<AgentClient>(client =>
            {
                client.BaseAddress = new Uri(new Uri(apiSettings.BaseUrl), "Agent/");
                if (!string.IsNullOrEmpty(apiSettings.ApiKeyHeaderName) && !string.IsNullOrEmpty(apiSettings.ApiKeyHeaderValue))
                {
                    client.DefaultRequestHeaders.Add(apiSettings.ApiKeyHeaderName, apiSettings.ApiKeyHeaderValue);
                }
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(5)
                };
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
        }

        var functionSettings = config.TryGetSection<ApiGatewaySettings>("AppSettings:FunctionGateway") ?? throw new InvalidOperationException("Missing AppSettings:FunctionGateway in configuration.");
        if (!string.IsNullOrEmpty(functionSettings.BaseUrl))
        {
            services.AddHttpClient<FunctionAppClient>(client =>
            {
                client.BaseAddress = new Uri(functionSettings.BaseUrl);
                if (!string.IsNullOrEmpty(functionSettings.ApiKeyHeaderName) && !string.IsNullOrEmpty(functionSettings.ApiKeyHeaderValue))
                {
                    client.DefaultRequestHeaders.Add(functionSettings.ApiKeyHeaderName, functionSettings.ApiKeyHeaderValue);
                }
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new SocketsHttpHandler
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(5)
                };
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
        }
    }
}
