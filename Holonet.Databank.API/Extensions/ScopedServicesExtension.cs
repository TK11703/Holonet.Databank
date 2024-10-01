using Holonet.Databank.Application.Services;
using Holonet.Databank.Infrastructure.Data;
using Holonet.Databank.Infrastructure.Repositories;

namespace Holonet.Databank.API.Extensions;

public static class ScopedServicesExtension
{
	public static IServiceCollection AddScopedServices(this IServiceCollection services)
	{
		services.AddScoped<ISqlDataAccess, SqlDataAccess>();

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

		return services;
	}
}
