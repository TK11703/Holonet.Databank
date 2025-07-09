using Holonet.Databank.AppFunctions.Clients;
using Holonet.Databank.AppFunctions.Configuration;
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
        var apiSettings = config.TryGetSection<ApiGatewaySettings>("AppSettings:ApiGateway");
        if (apiSettings == null)
            throw new InvalidOperationException("Missing AppSettings:ApiGateway in configuration.");
        

        if (!string.IsNullOrEmpty(apiSettings.BaseUrl))
        {
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

            services.AddHttpClient<AIServiceClient>(client =>
            {
                client.BaseAddress = new Uri(new Uri(apiSettings.BaseUrl), "AIServices/");
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
    }
}
