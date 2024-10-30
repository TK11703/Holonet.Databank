using System.Security.Claims;
using Holonet.Databank.Web.Clients;
using Holonet.Databank.Web.Models;
using Microsoft.Graph.Models;
using Microsoft.Identity.Web;

namespace Holonet.Databank.Web.Services;

public class AuthorMaintenanceService
{
	private readonly ILogger<AuthorMaintenanceService> _logger;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly AuthorClient _authorClient;


	public AuthorMaintenanceService(ILogger<AuthorMaintenanceService> logger, IHttpContextAccessor httpContextAccessor, AuthorClient authorClient)
	{
		_logger = logger;
		_httpContextAccessor = httpContextAccessor;
		_authorClient = authorClient;
	}

	public async Task AddOrUpdateAuthenticatedUser(string displayName, string email)
	{
		var user = _httpContextAccessor.HttpContext?.User;
		if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
		{
			Guid azureId;
			if (Guid.TryParse(user.GetObjectId(), out azureId))
			{
				if(string.IsNullOrEmpty(displayName))
                {
                    displayName = user.GetDisplayName() ?? string.Empty;
                }
				if(string.IsNullOrEmpty(email))
				{
					email = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
                }
				await Handle(azureId, displayName, email);
            }
			else
			{
				_logger.LogError("AzureId is not a valid Guid.");
			}
		}
		else
		{
			_logger.LogInformation("User/identity is null or not yet authenticated.");
		}
	}

    public async Task<string> GetBearerTokenForDiagnostic()
    {
        return await _authorClient.GetBearerToken();
	}

	private async Task Handle(Guid azureId, string? displayName, string? email)
	{
		var author = await _authorClient.Get(azureId);
        if (author == null)
        {
            var newAuthor = new AuthorModel()
            {
                AzureId = azureId,
                DisplayName = displayName ?? string.Empty,
                Email = email
            };
            if (await _authorClient.Create(newAuthor) > 0)
            {
                _logger.LogInformation("Author created successfully.");
            }
            else
            {
                _logger.LogError("Author creation failed.");
            }
        }
        else
        {
            if (!(author.DisplayName.Equals(displayName) && !string.IsNullOrEmpty(author.Email) && author.Email.Equals(email)))
            {
                author.DisplayName = displayName ?? string.Empty;
                author.Email = email;
                if (await _authorClient.Update(author, author.Id))
                {
                    _logger.LogInformation("Author updated successfully.");
                }
                else
                {
                    _logger.LogError("Author update failed.");
                }
            }
            else
            {
                _logger.LogInformation("Author update skipped, since no changes were detected.");
            }
        }
    }
}
