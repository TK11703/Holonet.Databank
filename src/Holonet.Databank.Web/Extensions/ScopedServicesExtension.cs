using Holonet.Databank.Web.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Identity.Web;

namespace Holonet.Databank.Web.Extensions;

public static class ScopedServicesExtension
{
	public static IServiceCollection AddScopedServices(this IServiceCollection services)
	{
		services.AddScoped<AuthorMaintenanceService>();
		services.AddScoped<UserService>();
        services.AddScoped<ILayoutService, LayoutService>();
        services.AddHealthChecks();
        return services;
	}
}
