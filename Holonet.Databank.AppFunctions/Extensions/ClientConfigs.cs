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

        if (!string.IsNullOrEmpty(baseApiAddress))
        {
			services.AddHttpClient<CharacterClient>(client =>
            {
                client.BaseAddress = new Uri(new Uri(baseApiAddress), "Characters/");
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
