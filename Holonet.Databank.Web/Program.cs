using Blazored.Toast;
using Holonet.Databank.Core;
using Holonet.Databank.Web.Components;
using Holonet.Databank.Web.Extensions;
using Holonet.Databank.Web.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
	.AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
	.EnableTokenAcquisitionToCallDownstreamApi(options => { builder.Configuration.Bind("AzureAd", options); })
	.AddInMemoryTokenCaches();

builder.Services.AddAuthorization(options =>
{
	options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
	options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
	options.TokenValidationParameters.NameClaimType = ClaimTypes.Name;
});
builder.Services.AddAuthorization(options =>
{
	AuthorizationPolicy defaultPolicy = new AuthorizationPolicyBuilder("Bearer")
        .RequireAuthenticatedUser()
		.RequireRole(ApplicationRole.User)
        .Build();
	options.DefaultPolicy = defaultPolicy;
    options.AddPolicy(AuthorizationPolicies.AssignmentToAdminRoleRequired, policy => policy.RequireClaim(ClaimTypes.Role, ApplicationRole.Administrator));
});

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents()
	.AddMicrosoftIdentityConsentHandler();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews(options =>
{
	var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
	options.Filters.Add(new AuthorizeFilter(policy));
}).AddMicrosoftIdentityUI();

builder.Services.AddScopedServices();

var graphScopes = builder.Configuration.GetValue<string>("MicrosoftGraph:Scopes")?.Split(' ');
if (graphScopes == null || graphScopes.Length == 0)
{
	graphScopes = ["user.read"];
}
var allowedHosts = builder.Configuration.GetValue<string>("AllowedHosts")?.Split(';');


builder.Services.AddGraphClient(builder.Configuration.GetValue<string>("MicrosoftGraph:GraphApiUrl"), graphScopes, allowedHosts);

builder.Services.ConfigureClients(builder.Configuration); //custom service extension method with configurations

builder.Services.AddBlazoredToast();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.MapHealthChecks("health");

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();


app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.MapControllers();

app.Run();
