using Microsoft.Kiota.Abstractions.Authentication;
using System.Security.Claims;

namespace Holonet.Databank.Web.Extensions;

public static class AllowedHostsValidatorExtension
{
	public static bool IsHostValid(this AllowedHostsValidator allowedHostsValidator, ClaimsPrincipal? claimsPrincipal)
	{
		if (allowedHostsValidator.AllowedHosts == null)
			return true;
		if (allowedHostsValidator.AllowedHosts.Count() == 1 && allowedHostsValidator.AllowedHosts.Contains("*"))
			return true;
		if(claimsPrincipal == null)
			return false;
		var host = claimsPrincipal.FindFirst("iss")?.Value; // Assuming the host is stored in the "iss" claim
		if (string.IsNullOrEmpty(host))
			return false;

		return allowedHostsValidator.AllowedHosts.Contains(host);
	}
}
