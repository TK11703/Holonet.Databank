﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Identity.Web;
using System.Security.Claims;

namespace Holonet.Databank.Web.Components.Shared;

/// <summary>
/// Base class for UserClaims component.
/// Retrieves claims present in the ID Token issued by Azure AD.
/// </summary>
public class UserClaimsBase : ComponentBase
{
    // AuthenticationStateProvider service provides the current user's ClaimsPrincipal data.
    [Inject]
    private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

    protected string? _authMessage;
    protected IEnumerable<Claim> _claims = [];

    // Defines list of claim types that will be displayed after successfull sign-in.
    private readonly string[] printClaims = { "name", "preferred_username", "tid", "oid", "email" , ClaimConstants.ObjectId, ClaimTypes.Email};

    protected override async Task OnInitializedAsync()
    {
        await GetClaimsPrincipalData();
    }

    /// <summary>
    /// Retrieves user claims for the signed-in user.
    /// </summary>
    /// <returns></returns>
    private async Task GetClaimsPrincipalData()
    {
        // Gets an AuthenticationState that describes the current user.
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

        var user = authState.User;

        // Checks if the user has been authenticated.
        if (user.Identity != null && user.Identity.IsAuthenticated)
        {
            _authMessage = $"{user.Identity.Name} is authenticated.";

            // Sets the claims value in _claims variable.
            // The claims mentioned in printClaims variable are selected only.
            _claims = user.Claims.Where(x => printClaims.Contains(x.Type));
		}
        else
        {
            _authMessage = "The user is NOT authenticated.";
        }
    }
}
