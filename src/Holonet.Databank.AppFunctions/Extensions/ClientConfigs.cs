using Holonet.Databank.AppFunctions.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Holonet.Databank.AppFunctions.Extensions;

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

        string? baseApiAddress = config.GetValue<string>("DatabankApiUrl");
        string? apiAuthKeyName = config.GetValue<string>("DatabankApim:KeyName");
        string? apiAuthKeyValue = config.GetValue<string>("DatabankApim:KeyValue");

        if (!string.IsNullOrEmpty(baseApiAddress))
        {
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

            services.AddHttpClient<AIServiceClient>(client =>
            {
                client.BaseAddress = new Uri(new Uri(baseApiAddress), "AIServices/");
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
    }
}
