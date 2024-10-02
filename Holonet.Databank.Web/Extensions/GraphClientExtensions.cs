using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.Kiota.Abstractions.Authentication;
using IAccessTokenProvider = Microsoft.Kiota.Abstractions.Authentication.IAccessTokenProvider;

namespace Holonet.Databank.Web.Extensions;

internal static class GraphClientExtensions
{
	public static IServiceCollection AddGraphClient(this IServiceCollection services, string? baseUrl, IEnumerable<string>? scopes, IEnumerable<string>? allowedHosts)
	{
		services.AddSingleton<AllowedHostsValidator>(sp => new AllowedHostsValidator(allowedHosts));

		services.AddScoped<CustomAccessTokenProvider>(sp =>
		{
			var tokenAquisition = sp.GetRequiredService<ITokenAcquisition>();
			var allowedHostsValidator = sp.GetRequiredService<AllowedHostsValidator>();
			return new CustomAccessTokenProvider(tokenAquisition, allowedHostsValidator, scopes);
		});
		services.AddScoped<GraphServiceClient>(sp =>
		{
			var accessTokenProvider = sp.GetRequiredService<CustomAccessTokenProvider>();
			var authenticationProvider = new BaseBearerTokenAuthenticationProvider(accessTokenProvider);
			return new GraphServiceClient(authenticationProvider, baseUrl);
		});

		return services;
	}
	public class CustomAccessTokenProvider(ITokenAcquisition tokenAcquisition, AllowedHostsValidator allowedHostsValidator, IEnumerable<string>? scopes) : IAccessTokenProvider
	{
		private readonly ITokenAcquisition _tokenAcquisition = tokenAcquisition;
		private readonly AllowedHostsValidator _allowedHostsValidator = allowedHostsValidator;
		private readonly IEnumerable<string>? _scopes = scopes;

		public AllowedHostsValidator AllowedHostsValidator => _allowedHostsValidator;

		public async Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
		{
			if(_scopes != null)
			{
				return await _tokenAcquisition.GetAccessTokenForUserAsync(_scopes);
			}
			else
			{
				throw new InvalidOperationException("Scopes are not set.");
			}
		}
	}
}