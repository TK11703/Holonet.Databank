using Microsoft.Identity.Web;
using System.Net.Http.Headers;

namespace Holonet.Databank.Web.Clients;

public class ClientBase
{
	private readonly ITokenAcquisition _tokenAcquisition;
	private readonly IEnumerable<string> _scopes;
	private readonly bool _requiresBearerToken;

	public ClientBase(ITokenAcquisition tokenAcquisition, IConfiguration configuration)
	{
		_tokenAcquisition = tokenAcquisition;
		_scopes = GetScopesFromConfiguration(configuration);
		_requiresBearerToken = configuration.GetValue<bool>("DatabankApi:RequiresBearerToken");
	}

	protected static IEnumerable<string> GetScopesFromConfiguration(IConfiguration configuration)
	{
		var section = configuration.GetSection("DatabankApi:Scopes");
		if (section.Exists())
		{
			return section.Get<string>()?.Split(' ') ?? Array.Empty<string>();
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
