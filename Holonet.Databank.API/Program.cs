using System.Reflection;
using Microsoft.AspNetCore.Diagnostics;
using Holonet.Databank.API.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Add Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

var scopes = builder.Configuration.GetValue<string>("AzureAd:Scopes")?.Split(' ');
if (scopes == null || scopes.Length == 0)
{
	scopes = ["access_as_user"];
}
// Add services to the container.
builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("AccessAsUserPolicy", policy =>
		policy.RequireAuthenticatedUser().RequireClaim("scp", scopes));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScopedServices();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

//Registers the endpoints that implement the IEndpoint interface
builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

var app = builder.Build();

var logger = app.Services.GetService<ILogger<Program>>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler(appError =>
{
	appError.Run(async context =>
	{
		context.Response.StatusCode = 500;
		context.Response.ContentType = "application/json";
		var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
		if (contextFeature is not null)
		{
			//log the error
			string error = $"Error: {contextFeature.Error}";
			if (logger is not null)
			{
				logger.LogError(error);
			}
			await context.Response.WriteAsJsonAsync(new
			{
				context.Response.StatusCode,
				Message = "Internal Server Error."
			});
		}
	});
});

app.UseAuthentication();

app.UseAuthorization();

//Register the endpoint as services in the application for use.
app.MapEndpoints();

app.Run();