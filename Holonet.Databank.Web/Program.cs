using Blazored.Toast;
using Holonet.Databank.Core;
using Holonet.Databank.Web.Components;
using Holonet.Databank.Web.Extensions;
using Holonet.Databank.Web.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.Kiota.Abstractions.Authentication;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

var graphScopes = builder.Configuration.GetValue<string>("MicrosoftGraph:Scopes")?.Split(' ');
if (graphScopes == null || graphScopes.Length == 0)
{
	graphScopes = ["user.read"];
}
var databankApiScopes = builder.Configuration["DatabankApi:Scopes"]?.Split(' ');
var allowedHosts = builder.Configuration.GetValue<string>("AllowedHosts")?.Split(';');

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
	.AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
	.EnableTokenAcquisitionToCallDownstreamApi(databankApiScopes)
	.AddDownstreamApi("DatabankApi", builder.Configuration.GetSection("DatabankApi"))
	.AddInMemoryTokenCaches();


builder.Services.AddAuthorization(options =>
{
	options.FallbackPolicy = options.DefaultPolicy;
});
//var graphScopes = builder.Configuration.GetValue<string>("MicrosoftGraph:Scopes")?.Split(' ');
//if (graphScopes == null || graphScopes.Length == 0)
//{
//	graphScopes = ["user.read"];
//}
//string[]? tokenAcquisitionScopes = builder.Configuration.GetValue<string>("TokenAquisitionScopes")?.Split(' ');
//var allowedHosts = builder.Configuration.GetValue<string>("AllowedHosts")?.Split(';');

//JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

//builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
//	.AddMicrosoftIdentityWebApp(options =>
//	{
//		builder.Configuration.Bind("AzureAd", options);
//		options.Events = new OpenIdConnectEvents
//		{
//			OnTokenValidated = async context =>
//			{
//				// Custom logic for token validation
//				var allowedHostsValidator = context.HttpContext.RequestServices.GetRequiredService<AllowedHostsValidator>();
//				if (allowedHostsValidator != null)
//				{
//					if(!allowedHostsValidator.IsHostValid(context.Principal))
//					{
//						context.Fail("Unauthorized host.");
//					}
//					await Task.CompletedTask;	
//				}
//			}
//		};
//	})
//	.EnableTokenAcquisitionToCallDownstreamApi(tokenAcquisitionScopes)
//	//.AddDownstreamApi("Databank.Api", builder.Configuration.GetSection("DatabankApi"))
//    .AddInMemoryTokenCaches();

//builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
//{
//	options.TokenValidationParameters.RoleClaimType = "roles";
//	options.TokenValidationParameters.NameClaimType = "name";
//});
//builder.Services.AddAuthorization(options =>
//{
//	options.AddPolicy(AuthorizationPolicies.AssignmentToAdminRoleRequired, policy => policy.RequireClaim("roles", ApplicationRole.Administrator));
//});
//builder.Services.Configure<CookiePolicyOptions>(options =>
//{
//	// This lambda determines whether user consent for non-essential cookies is needed for a given request.
//	options.CheckConsentNeeded = context => false;
//	// requires using Microsoft.AspNetCore.Http;
//	options.MinimumSameSitePolicy = SameSiteMode.None;
//	// Handling SameSite cookie according to https://docs.microsoft.com/en-us/aspnet/core/security/samesite
//	options.HandleSameSiteCookieCompatibility();
//});
// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents()
	.AddMicrosoftIdentityConsentHandler();

//builder.Services.AddCascadingAuthenticationState();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    options.Filters.Add(new AuthorizeFilter(policy));
}).AddMicrosoftIdentityUI();

builder.Services.AddScoped<AuthorMaintenanceService>();

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

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.MapControllers();

app.Run();