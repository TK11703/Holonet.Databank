using System.Reflection;
using Microsoft.AspNetCore.Diagnostics;
using Holonet.Databank.API.Extensions;
using FluentValidation;
using Holonet.Databank.API.Middleware;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddLogging();
builder.Services.AddHttpContextAccessor();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

builder.Services.AddScopedServices(configuration);

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

//Registers the endpoints that implement the IEndpoint interface
builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

builder.Services.AddHostedService<ChatHistoryCleanupService>();

var app = builder.Build();

var logger = app.Services.GetService<ILogger<Program>>();

var showSwagger = builder.Configuration.GetValue<bool>("ShowSwagger");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || showSwagger)
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapHealthChecks("health", new HealthCheckOptions
{
	Predicate = _ => true,
	ResponseWriter = HealthCheckResponseWriter.WriteResponse
});

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

//Register the endpoint as services in the application for use.
app.MapEndpoints();

app.Run();
