using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Holonet.Databank.Application.Services;
public class UserService(IHttpContextAccessor httpContextAccessor) : IUserService
{
	private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;


	public Guid? GetAzureId()
	{
		var user = _httpContextAccessor.HttpContext.User;
		var oidClaim = user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
		if (Guid.TryParse(oidClaim, out Guid oid))
		{
			return oid;
		}
		else
		{
			return null;
		}
	}
}
