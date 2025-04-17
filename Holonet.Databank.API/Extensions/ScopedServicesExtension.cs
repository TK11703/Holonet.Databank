using Holonet.Databank.Application.Services;
using Holonet.Databank.Infrastructure.Data;
using Holonet.Databank.Infrastructure.Repositories;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using Holonet.Databank.Application.AICapabilities;
using Holonet.Databank.Application.AICapabilities.Plugins;
using Holonet.Databank.Infrastructure.Health;
using Azure;
using Azure.Search.Documents.Indexes;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.SemanticKernel.Embeddings;


namespace Holonet.Databank.API.Extensions;

public static class ScopedServicesExtension
{
	public static IServiceCollection AddScopedServices(this IServiceCollection services, ConfigurationManager configuration)
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

#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        services.AddAzureOpenAITextEmbeddingGeneration(
            deploymentName: configuration.GetValue<string>("AzureOpenAi:EmbeddingModel")!,
            endpoint: configuration.GetValue<string>("AzureOpenAi:Endpoint")!,
            apiKey: configuration.GetValue<string>("AzureOpenAi:ApiKey")!
            );
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        services.AddSingleton<SearchIndexClient>(sp =>
                new SearchIndexClient(
                    endpoint: new Uri(configuration.GetValue<string>("AzureAiSearch:Endpoint")!),
                    credential: new AzureKeyCredential(configuration.GetValue<string>("AzureAiSearch:ApiKey")!)
                    )
                );

        services.AddTransient<Kernel>(sp =>
		{
			var kernelBuilder = Kernel.CreateBuilder();

            kernelBuilder.Services.AddSingleton<IConfiguration>(configuration);

            kernelBuilder.AddAzureOpenAIChatCompletion( 
				deploymentName: configuration.GetValue<string>("AzureOpenAi:Model")!,
				endpoint: configuration.GetValue<string>("AzureOpenAi:Endpoint")!,
				apiKey: configuration.GetValue<string>("AzureOpenAi:ApiKey")!
				);

            
#pragma warning disable SKEXP0020 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            kernelBuilder.AddAzureAISearchVectorStore();
#pragma warning disable SKEXP0020 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            kernelBuilder.Plugins.AddFromType<UtcPlugin>("UTCTime");
            kernelBuilder.Plugins.AddFromObject(new GeocodingPlugin(sp.GetRequiredService<IHttpClientFactory>(), configuration), "GeocodingPlugin");
            kernelBuilder.Plugins.AddFromObject(new WeatherPlugin(sp.GetRequiredService<IHttpClientFactory>()), "WeatherPlugin");

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            //kernelBuilder.Plugins.AddFromObject(new HolonetSearchPlugin(sp.GetRequiredService<ITextEmbeddingGenerationService>(), sp.GetRequiredService<SearchIndexClient>(), configuration), "HolonetSearchPlugin");
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            kernelBuilder.Plugins.AddFromType<HolonetSearchPlugin>("HolonetSearchPlugin");
            kernelBuilder.Plugins.AddFromObject(new BingSearchPlugin(sp.GetRequiredService<IHttpClientFactory>(), configuration), "BingSearchPlugin");
            return kernelBuilder.Build();
		});

		services.AddSingleton<IChatCompletionService>(sp => sp.GetRequiredService<Kernel>().GetRequiredService<IChatCompletionService>());

		services.AddSingleton<IChatHistoryManager>(sp =>
		{
			return new ChatHistoryManager(CorePrompts.GetSystemPrompt());
		});

		return services;
	}
}
