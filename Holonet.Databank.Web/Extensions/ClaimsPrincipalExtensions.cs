using Microsoft.Graph.Models;
using System.Security.Claims;
using Holonet.Databank.Core;

namespace Holonet.Databank.Web.Extensions;

public static class ClaimsPrincipalExtensions
{
	/// <summary>
	/// Gets the user's display name from the Microsoft Graph Display Name claim.
	/// </summary>
	/// <param name="claimsPrincipal"></param>
	/// <returns></returns>
	public static string? GetUserGraphDisplayName(this ClaimsPrincipal claimsPrincipal)
	{
		return claimsPrincipal.FindFirstValue(GraphClaimTypes.DisplayName);
	}

	/// <summary>
	/// Gets the user's display name from the Azure AD Name claim.
	/// </summary>
	/// <param name="claimsPrincipal"></param>
	/// <returns></returns>
	public static string? GetUserDisplayName(this ClaimsPrincipal claimsPrincipal)
	{
		return claimsPrincipal.FindFirstValue(AzureClaimTypes.DisplayName);
	}

	/// <summary>
	/// Gets the user's email from the default claim or the Azure AD Name claim .
	/// </summary>
	/// <param name="claimsPrincipal"></param>
	/// <returns></returns>
	public static string? GetUserEmail(this ClaimsPrincipal claimsPrincipal)
	{
		var email = string.Empty;
		if (!string.IsNullOrEmpty(claimsPrincipal.FindFirstValue(ClaimTypes.Email)))
		{
			email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
		}
		else if (!string.IsNullOrEmpty(claimsPrincipal.FindFirstValue(AzureClaimTypes.Email)))
		{
			email = claimsPrincipal.FindFirstValue(AzureClaimTypes.Email);
		}
		return email;
	}

	/// <summary>
	/// Gets the user's preferred username from the Azure claim.
	/// </summary>
	/// <param name="claimsPrincipal"></param>
	/// <returns></returns>
	public static string? GetUserName(this ClaimsPrincipal claimsPrincipal)
	{
		return claimsPrincipal.FindFirstValue(AzureClaimTypes.PreferredUsername);
	}

	/// <summary>
	/// Gets the user's Id from the Microsoft Graph's claim.
	/// </summary>
	/// <param name="claimsPrincipal"></param>
	/// <returns></returns>
	public static string? GetUserGraphId(this ClaimsPrincipal claimsPrincipal)
	{
		return claimsPrincipal.FindFirstValue(GraphClaimTypes.Id);
	}

	/// <summary>
	/// Gets the user's email from the Microsoft Graph's claim
	/// </summary>
	/// <param name="claimsPrincipal"></param>
	/// <returns></returns>
	public static string? GetUserGraphEmail(this ClaimsPrincipal claimsPrincipal)
	{
		return claimsPrincipal.FindFirstValue(GraphClaimTypes.Email);
	}

	public static string? GetUserGraphPhoto(this ClaimsPrincipal claimsPrincipal)
	{
		return claimsPrincipal.FindFirstValue(GraphClaimTypes.Photo);
	}

	/// <summary>
	/// Gets the user's mobile number from the Microsoft Graph's claim
	/// </summary>
	/// <param name="claimsPrincipal"></param>
	/// <returns></returns>
	public static string? GetUserGraphMobile(this ClaimsPrincipal claimsPrincipal)
	{
		return claimsPrincipal.FindFirstValue(GraphClaimTypes.Mobile);
	}

	/// <summary>
	/// Adds the user's Microsoft Graph information to the claims principal.
	/// </summary>
	/// <param name="claimsPrincipal"></param>
	/// <param name="user"></param>
	/// <exception cref="Exception"></exception>
	public static void AddUserGraphInfo(this ClaimsPrincipal claimsPrincipal, User user)
	{
		var identity = claimsPrincipal.Identity as ClaimsIdentity;
		if (identity == null) throw new ArgumentNullException(nameof(claimsPrincipal), "Unable to add Graph information to user identity, because the claims principal user identity was invalid.");
		if (user == null) throw new ArgumentNullException(nameof(user), "Unable to add Graph information to user identity, because the Graph user was invalid.");

		if (!identity.HasClaim(c => c.Type == GraphClaimTypes.Id) && !string.IsNullOrEmpty(user.Id))
		{
			identity.AddClaim(new Claim(GraphClaimTypes.Id, user.Id));
		}
		if (!identity.HasClaim(c => c.Type == GraphClaimTypes.DisplayName) && !string.IsNullOrEmpty(user.DisplayName))
		{
			identity.AddClaim(new Claim(GraphClaimTypes.DisplayName, user.DisplayName));
		}
		if (!identity.HasClaim(c => c.Type == GraphClaimTypes.Email))
		{
			if (!string.IsNullOrEmpty(user.Mail))
			{
				identity.AddClaim(new Claim(GraphClaimTypes.Email, user.Mail));
			}
			else if (!string.IsNullOrEmpty(user.UserPrincipalName))
			{
				identity.AddClaim(new Claim(GraphClaimTypes.Email, user.UserPrincipalName));
			}
		}	
		if (!identity.HasClaim(c => c.Type == GraphClaimTypes.Mobile) && !string.IsNullOrEmpty(user.MobilePhone))
		{
			identity.AddClaim(new Claim(GraphClaimTypes.Mobile, user.MobilePhone));
		}
	}
}