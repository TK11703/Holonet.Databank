using Azure;
using Azure.Search.Documents.Indexes;
using Holonet.Databank.API.Configuration;
using Holonet.Databank.Application.AICapabilities;
using Holonet.Databank.Application.AICapabilities.Plugins;
using Holonet.Databank.Application.Services;
using Holonet.Databank.Infrastructure.Data;
using Holonet.Databank.Infrastructure.Repositories;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Holonet.Databank.API.Extensions;

public static class ScopedServicesExtension
{
    public static IServiceCollection AddScopedServices(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddScoped<ISqlDataAccess, SqlDataAccess>();

        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();

        services.AddScoped<IAliasRepository, AliasRepository>();

        services.AddScoped<IDataRecordService, DataRecordService>();
        services.AddScoped<IDataRecordRepository, DataRecordRepository>();

        services.AddScoped<ICharacterService, CharacterService>();
        services.AddScoped<ICharacterRepository, CharacterRepository>();

        services.AddScoped<ICharacterService, CharacterService>();
        services.AddScoped<ICharacterSpeciesRepository, CharacterSpeciesRepository>();

        services.AddScoped<IPlanetService, PlanetService>();
        services.AddScoped<IPlanetRepository, PlanetRepository>();

        services.AddScoped<ISpeciesService, SpeciesService>();
        services.AddScoped<ISpeciesRepository, SpeciesRepository>();

        services.AddScoped<IHistoricalEventService, HistoricalEventService>();
        services.AddScoped<IHistoricalEventRepository, HistoricalEventRepository>();
        services.AddScoped<IHistoricalEventCharacterRepository, HistoricalEventCharacterRepository>();
        services.AddScoped<IHistoricalEventPlanetRepository, HistoricalEventPlanetRepository>();

        services.AddHealthChecks();
        //.AddCheck<DatabaseHealthCheck>("database", HealthStatus.Unhealthy);

        services.AddHttpClient();

        services.AddAzureOpenAIChatCompletion(
                deploymentName: appSettings.AzureOpenAi.Model!,
                endpoint: appSettings.AzureOpenAi.Endpoint!,
                apiKey: appSettings.AzureOpenAi.ApiKey!
                );


#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        services.AddAzureOpenAIEmbeddingGenerator(
            deploymentName: appSettings.AzureOpenAi.EmbeddingModel!,
            endpoint: appSettings.AzureOpenAi.Endpoint!,
            apiKey: appSettings.AzureOpenAi.ApiKey!,
            modelId: appSettings.AzureOpenAi.Model!
        );
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        services.AddSingleton<SearchIndexClient>(sp =>
        {
            var endpoint = new Uri(appSettings.AzureAiSearch.Endpoint!);
            var credential = new AzureKeyCredential(appSettings.AzureAiSearch.ApiKey!);
            return new SearchIndexClient(endpoint, credential);
        });

        services.AddTransient<Kernel>(sp =>
        {
            var kernelBuilder = Kernel.CreateBuilder();

            kernelBuilder.Services.AddAzureAISearchVectorStore();

            kernelBuilder.Plugins.AddFromType<UtcPlugin>("UTCTime");

            kernelBuilder.Plugins.AddFromObject(new GeocodingPlugin(sp.GetRequiredService<IHttpClientFactory>(), appSettings.GeoCodingApiKey), "GeocodingPlugin");
            kernelBuilder.Plugins.AddFromObject(new WeatherPlugin(sp.GetRequiredService<IHttpClientFactory>()), "WeatherPlugin");
            kernelBuilder.Plugins.AddFromObject(new HolonetSearchPlugin(
                sp.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>(),
                sp.GetRequiredService<SearchIndexClient>(),
                appSettings.AzureAiSearch.Index), "HolonetSearchPlugin");

            return kernelBuilder.Build();
        });

        services.AddSingleton<IChatHistoryManager>(sp =>
        {
            return new ChatHistoryManager(CorePrompts.GetSystemPrompt());
        });

        return services;
    }
}
