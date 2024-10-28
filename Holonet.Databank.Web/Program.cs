using Blazored.Toast;
using Holonet.Databank.Core;
using Holonet.Databank.Web.Components;
using Holonet.Databank.Web.Extensions;
using Holonet.Databank.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

var graphScopes = builder.Configuration.GetValue<string>("MicrosoftGraph:Scopes")?.Split(' ');
if (graphScopes == null || graphScopes.Length == 0)
{
	graphScopes = ["user.read"];
}
var allowedHosts = builder.Configuration.GetValue<string>("AllowedHosts")?.Split(';');

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


app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.MapControllers();

app.Run();