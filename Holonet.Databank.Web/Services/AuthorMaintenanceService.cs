using System.Security.Claims;
using Holonet.Databank.Web.Clients;
using Holonet.Databank.Web.Models;
using Microsoft.Graph.Models;
using Microsoft.Identity.Web;

namespace Holonet.Databank.Web.Services;

public class AuthorMaintenanceService
{
	private readonly ILogger<AuthorMaintenanceService> _logger;
	private readonly AuthorClient _authorClient;
	private readonly UserService _userService;


	public AuthorMaintenanceService(ILogger<AuthorMaintenanceService> logger, AuthorClient authorClient, UserService userService)
	{
		_logger = logger;
		_authorClient = authorClient;
		_userService = userService;
	}

	public async Task AddOrUpdateAuthenticatedUser(string displayName, string email)
	{
		if (_userService.IsUserAuthenticated())
		{
			Guid azureId = _userService.GetAzureId();
			if (string.IsNullOrEmpty(displayName))
			{
				displayName = _userService.GetUserDisplayName() ?? string.Empty;
			}
			if (string.IsNullOrEmpty(email))
			{
				email = _userService.GetUserEmail() ?? string.Empty;
			}
			await Handle(azureId, displayName, email);
		}
		else
		{
			_logger.LogInformation("User/identity is null or not yet authenticated.");
		}
	}

	public async Task<string> GetBearerTokenForDiagnostic()
	{
		if (_authorClient.RequiresBearToken())
		{
			return await _authorClient.GetBearerToken();
		}
		return "The application is currently configured to ignore bearer tokens, so this request has been skipped";
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
