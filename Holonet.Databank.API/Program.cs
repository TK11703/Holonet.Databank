using System.Reflection;
using Microsoft.AspNetCore.Diagnostics;
using Holonet.Databank.API.Extensions;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddAuthorization();

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

app.UseAuthorization();

//Register the endpoint as services in the application for use.
app.MapEndpoints();

app.Run();