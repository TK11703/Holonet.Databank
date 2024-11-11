using Holonet.Databank.Application.Services;
using Holonet.Databank.Infrastructure.Data;
using Holonet.Databank.Infrastructure.Repositories;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using Holonet.Databank.Application.AICapabilities;
using Holonet.Databank.Application.AICapabilities.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

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

		services.AddHttpClient();

		services.AddTransient<Kernel>(sp =>
		{
			var builder = Kernel.CreateBuilder();
			builder.AddAzureOpenAIChatCompletion(
				deploymentName: configuration.GetValue<string>("AzureOpenAi:Model"),
				endpoint: configuration.GetValue<string>("AzureOpenAi:Endpoint"),
				apiKey: configuration.GetValue<string>("AzureOpenAi:ApiKey"));

			builder.Plugins.AddFromType<UtcPlugin>("UTCTime");
			builder.Plugins.AddFromObject(new GeocodingPlugin(sp.GetRequiredService<IHttpClientFactory>(), configuration), "GeocodingPlugin");
			builder.Plugins.AddFromObject(new WeatherPlugin(sp.GetRequiredService<IHttpClientFactory>(), configuration), "WeatherPlugin");
			return builder.Build();
		});

		services.AddSingleton<IChatCompletionService>(sp => sp.GetRequiredService<Kernel>().GetRequiredService<IChatCompletionService>());

		services.AddSingleton<IChatHistoryManager>(sp =>
		{
			return new ChatHistoryManager(CorePrompts.GetSystemPrompt());
		});

		return services;
	}
}
