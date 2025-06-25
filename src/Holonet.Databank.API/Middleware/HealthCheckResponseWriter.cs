using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace Holonet.Databank.API.Middleware;
public static class HealthCheckResponseWriter
{
    public static Task WriteResponse(HttpContext context, HealthReport result)
    {
        context.Response.ContentType = "application/json";

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(new
        {
            status = result.Status.ToString(),
            checks = result.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description
            })
        }, options);

        return context.Response.WriteAsync(json);
    }
}
