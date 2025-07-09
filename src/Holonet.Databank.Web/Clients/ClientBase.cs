using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using System.Net.Http.Headers;
using Holonet.Databank.Web.Configuration;

namespace Holonet.Databank.Web.Clients;

public class ClientBase(ITokenAcquisition tokenAcquisition, IOptions<AppSettings> options)
{
	private readonly ITokenAcquisition _tokenAcquisition = tokenAcquisition;
	private readonly IEnumerable<string> _scopes = GetScopesFromConfiguration(options.Value.ApiGateway);
	private readonly bool _requiresBearerToken = options.Value.ApiGateway.RequiresBearerToken;

    protected static IEnumerable<string> GetScopesFromConfiguration(ApiGatewaySettings apiSettings)
	{
		if(!string.IsNullOrWhiteSpace(apiSettings.Scopes))
		{
			return apiSettings.Scopes.Split(' ');
        }
		else
		{
            return Array.Empty<string>();
        }
	}

	public bool RequiresBearToken()
	{
		return _requiresBearerToken;
	}

	public async Task AcquireBearerTokenForClient(HttpClient httpClient)
	{
		var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(_scopes);
		if (!string.IsNullOrEmpty(accessToken))
		{
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
		}
	}

	public async Task<string> GetBearerToken()
	{
		var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(_scopes);
		if (!string.IsNullOrEmpty(accessToken))
		{
			return accessToken;
		}
		return string.Empty;
	}
}
