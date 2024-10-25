using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;

namespace Holonet.Databank.API;

public static class OpenIdConfigurationHelper
{
	public static OpenIdConnectConfiguration GetDiscoveryDocument(string authority)
	{
		var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
			$"{authority}/.well-known/openid-configuration",
			new OpenIdConnectConfigurationRetriever());

		return configurationManager.GetConfigurationAsync().Result;
	}
}
