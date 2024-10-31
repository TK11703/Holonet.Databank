using Holonet.Databank.Web.Extensions;
using Microsoft.Identity.Web;
using System.Security.Claims;

namespace Holonet.Databank.Web.Services;

public class UserService
{
	private readonly ILogger<UserService> _logger;
	private readonly IHttpContextAccessor _httpContextAccessor;

	public UserService(ILogger<UserService> logger, IHttpContextAccessor httpContextAccessor)
	{
		_logger = logger;
		_httpContextAccessor = httpContextAccessor;
	}

	public bool IsUserAuthenticated()
	{
		var user = _httpContextAccessor.HttpContext?.User;
		return user != null && user.Identity != null && user.Identity.IsAuthenticated;
	}

	public static bool IsUserAuthenticated(ClaimsPrincipal user)
	{
		return user != null && user.Identity != null && user.Identity.IsAuthenticated;
	}

	public Guid GetAzureId()
	{
		var user = _httpContextAccessor.HttpContext?.User;
		if (user == null)
		{
			_logger.LogInformation("User principal is null.");
			return Guid.Empty;
		}
		if (IsUserAuthenticated(user))
		{
			
			if (Guid.TryParse(user.GetObjectId(), out var azureId))
			{
				return azureId;
			}
			else
			{
				_logger.LogError("AzureId is not a valid Guid.");
			}
		}
		else
		{
			_logger.LogInformation("User/identity is not yet authenticated.");
		}
		return Guid.Empty;
	}

	public string? GetUserEmail()
	{
		var user = _httpContextAccessor.HttpContext?.User;
		if (user == null)
		{
			_logger.LogInformation("User principal is null.");
			return string.Empty;
		}
		return user.GetUserEmail();
	}

	public string? GetUserDisplayName()
	{
		var user = _httpContextAccessor.HttpContext?.User;
		if (user == null)
		{
			_logger.LogInformation("User principal is null.");
			return string.Empty;
		}
		return user.GetUserDisplayName();
	}
}