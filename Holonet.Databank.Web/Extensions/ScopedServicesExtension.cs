using Holonet.Databank.Web.Services;
using Microsoft.Identity.Web;

namespace Holonet.Databank.Web.Extensions;

public static class ScopedServicesExtension
{
	public static IServiceCollection AddScopedServices(this IServiceCollection services)
	{
		services.AddScoped<AuthorMaintenanceService>();
		services.AddScoped<UserService>();
		return services;
	}
}
