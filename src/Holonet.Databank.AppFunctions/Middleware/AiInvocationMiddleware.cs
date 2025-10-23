using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Holonet.Databank.AppFunctions.Middleware;

public class AiInvocationMiddleware : IFunctionsWorkerMiddleware
{
    private readonly TelemetryClient _telemetryClient;

    public AiInvocationMiddleware(TelemetryClient telemetryClient)
      => _telemetryClient = telemetryClient;

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();
        var funcName = context.FunctionDefinition.Name;

        try
        {
            await next(context);
            stopwatch.Stop();

            var telemetry = new RequestTelemetry
            {
                Name = funcName,
                Timestamp = DateTimeOffset.UtcNow - stopwatch.Elapsed,
                Duration = stopwatch.Elapsed,
                Success = true,
                ResponseCode = "0"
            };

            // correlate by invocation ID
            var invocationId = context.InvocationId;
            telemetry.Context.Operation.Id = invocationId;
            telemetry.Context.Operation.ParentId = invocationId;

            _telemetryClient.TrackRequest(telemetry);
            _telemetryClient.Flush();
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // failure telemetry
            var telemetry = new RequestTelemetry
            {
                Name = funcName,
                Timestamp = DateTimeOffset.UtcNow - stopwatch.Elapsed,
                Duration = stopwatch.Elapsed,
                Success = false,
                ResponseCode = "1"
            };

            _telemetryClient.TrackException(ex);
            _telemetryClient.TrackRequest(telemetry);
            _telemetryClient.Flush();

            throw;
        }
    }
}