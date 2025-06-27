using Holonet.Databank.Web.Clients;

namespace Holonet.Databank.Web.Extensions;

internal static class ClientConfigs
{
    /// <summary>
    /// Helps configure these TYPED clients (which are transient) to be used in potential singleton services and not be affected by DNS refresh issues and port exhaustion. Otherwise 
    /// NAMED clients could be used instead with the HttpClientFactory usage.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    public static void ConfigureClients(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddScoped<GraphApiClient>();

        string? baseApiAddress = config.GetValue<string>("DatabankApi:DeployedUrl");
        string? apiAuthKeyName = config.GetValue<string>("DatabankApim:KeyName");
        string? apiAuthKeyValue = config.GetValue<string>("DatabankApim:KeyValue");

        if (!string.IsNullOrEmpty(baseApiAddress))
        {
            services.AddHttpClient<AuthorClient>(client =>
            {
                client.BaseAddress = new Uri(new Uri(baseApiAddress), "Authors/");
                if(!string.IsNullOrEmpty(apiAuthKeyName) && !string.IsNullOrEmpty(apiAuthKeyValue))
                {
                    client.DefaultRequestHeaders.Add(apiAuthKeyName, apiAuthKeyValue);
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
                client.BaseAddress = new Uri(new Uri(baseApiAddress), "Characters/");
                if (!string.IsNullOrEmpty(apiAuthKeyName) && !string.IsNullOrEmpty(apiAuthKeyValue))
                {
                    client.DefaultRequestHeaders.Add(apiAuthKeyName, apiAuthKeyValue);
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
                client.BaseAddress = new Uri(new Uri(baseApiAddress), "Planets/");
                if (!string.IsNullOrEmpty(apiAuthKeyName) && !string.IsNullOrEmpty(apiAuthKeyValue))
                {
                    client.DefaultRequestHeaders.Add(apiAuthKeyName, apiAuthKeyValue);
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
                client.BaseAddress = new Uri(new Uri(baseApiAddress), "Species/");
                if (!string.IsNullOrEmpty(apiAuthKeyName) && !string.IsNullOrEmpty(apiAuthKeyValue))
                {
                    client.DefaultRequestHeaders.Add(apiAuthKeyName, apiAuthKeyValue);
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
                client.BaseAddress = new Uri(new Uri(baseApiAddress), "HistoricalEvents/");
                if (!string.IsNullOrEmpty(apiAuthKeyName) && !string.IsNullOrEmpty(apiAuthKeyValue))
                {
                    client.DefaultRequestHeaders.Add(apiAuthKeyName, apiAuthKeyValue);
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
                client.BaseAddress = new Uri(new Uri(baseApiAddress), "Agent/");
                if (!string.IsNullOrEmpty(apiAuthKeyName) && !string.IsNullOrEmpty(apiAuthKeyValue))
                {
                    client.DefaultRequestHeaders.Add(apiAuthKeyName, apiAuthKeyValue);
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

        string? baseFunctionAppAddress = config.GetValue<string>("DatabankFunctionApp:DeployedUrl");
        string? functionAppKey = config.GetValue<string>("DatabankFunctionApp:FunctionKey");

        if (!string.IsNullOrEmpty(baseFunctionAppAddress))
        {
            services.AddHttpClient<FunctionAppClient>(client =>
            {
                client.BaseAddress = new Uri(baseFunctionAppAddress);
                if (!string.IsNullOrEmpty(functionAppKey))
                {
                    client.DefaultRequestHeaders.Add("x-functions-key", functionAppKey);
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
