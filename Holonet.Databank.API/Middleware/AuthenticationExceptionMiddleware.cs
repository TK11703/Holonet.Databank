using System.Net;
using System.Security.Authentication;

namespace Holonet.Databank.API.Middleware;

public class AuthenticationExceptionMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<AuthenticationExceptionMiddleware> _logger;

	public AuthenticationExceptionMiddleware(RequestDelegate next, ILogger<AuthenticationExceptionMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (AuthenticationException ex)
		{
			_logger.LogError(ex, "An authentication exception occurred.");
			await HandleAuthenticationExceptionAsync(context, ex);
		}
	}

	private Task HandleAuthenticationExceptionAsync(HttpContext context, Exception exception)
	{
		context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
		context.Response.ContentType = "application/json";

		var responseMessage = new
		{
			Error = "Authentication failed",
			Details = exception.Message
		};

		return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(responseMessage));
	}
}

