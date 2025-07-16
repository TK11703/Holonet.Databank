using Holonet.Databank.AppFunctions.Clients;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Holonet.Databank.AppFunctions.Functions;

public class DBAwake(GenericDBClient genericDBClient, ILoggerFactory loggerFactory)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<DBAwake>();
    private readonly GenericDBClient _genericDBClient = genericDBClient;

    [Function("DBAwake")]
    public async Task Run([TimerTrigger("0 57 7 * * *")] TimerInfo myTimer)
    {
        DateTime executedOn = DateTime.UtcNow;
        _logger.LogInformation("Holonet.Databank.Functions DBAwake executed at: {ExecutionTime}", executedOn);

        try
        {
            bool dbReady = await _genericDBClient.IsDBReady();
            if (dbReady)
            {
                _logger.LogInformation("Holonet.Databank.Functions DBAwake: Database is ready.");
            }
            else
            {
                _logger.LogWarning("Holonet.Databank.Functions DBAwake: Database is not ready.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Holonet.Databank.Functions DBAwake error: Exception occurred while checking DB readiness. Executed at: {ExecutionTime}", executedOn);
            throw new InvalidOperationException($"Exception occurred in DBAwake at {executedOn}.", ex);
        }

        if (myTimer.ScheduleStatus is not null)
        {
            _logger.LogInformation("Holonet.Databank.Functions DBAwake next timer schedule at: {NextSchedule}", myTimer.ScheduleStatus.Next);
        }
    }
}