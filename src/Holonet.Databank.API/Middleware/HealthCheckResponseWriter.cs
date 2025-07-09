using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace Holonet.Databank.API.Middleware;
public static class HealthCheckResponseWriter
{
    private static readonly JsonSerializerOptions CachedJsonSerializerOptions = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    public static Task WriteResponse(HttpContext context, HealthReport result)
    {
        context.Response.ContentType = "application/json";

        var json = JsonSerializer.Serialize(new
        {
            status = result.Status.ToString(),
            checks = result.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description
            })
        }, CachedJsonSerializerOptions);

        return context.Response.WriteAsync(json);
    }
}
