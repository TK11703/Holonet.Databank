using Holonet.Databank.AppFunctions.Clients;
using Holonet.Databank.AppFunctions.Configuration;
using Holonet.Databank.Core.Dtos;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Holonet.Databank.AppFunctions.Functions;

public class NewShardInQueue(ILogger<NewShardInQueue> logger, TelemetryClient telemetryClient, ILoggerFactory loggerFactory, IOptions<AppSettings> appSettings, AIServiceClient serviceClient, CharacterClient characterClient, PlanetClient planetClient, SpeciesClient speciesClient, HistoricalEventClient historicalEventClient) 
    : DataRecordDtoProcessor(loggerFactory, appSettings, serviceClient, characterClient, planetClient, speciesClient, historicalEventClient)
{
    private readonly ILogger<NewShardInQueue> _logger = logger;
    private readonly AppSettings _settings = appSettings.Value;
    private readonly TelemetryClient _telemetryClient = telemetryClient;

    [Function("NewShardInQueue")]
    public async Task Run([QueueTrigger("shardprocessqueue", Connection = "StorageQueueConnection")] string message)
    {
        var requestTelemetry = new RequestTelemetry
        {
            Name = "NewShardInQueue",
            Timestamp = DateTimeOffset.UtcNow,
            Duration = TimeSpan.FromSeconds(1), // Adjust as needed
            Success = true
        };

        _telemetryClient.TrackRequest(requestTelemetry);

        DateTime executedOn = DateTime.UtcNow;
        _logger.LogInformation("Holonet.Databank.Functions NewShardInQueue was triggered at: {ExecutionTime}.", executedOn);

        if (_settings.UseQueueTrigger)
        {
            DataRecordFunctionDto? record = null;
            try
            {
                record = JsonSerializer.Deserialize<DataRecordFunctionDto?>(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Holonet.Databank.Functions NewShardInQueue error: Failed to deserialize message.");
                return;
            }

            if (record == null)
            {
                _logger.LogError("Holonet.Databank.Functions NewShardInQueue error: Invalid data record.");
            }
            else if (record.Id.Equals(0))
            {
                _logger.LogError("Holonet.Databank.Functions NewShardInQueue error: Invalid data record - no item ID.");
            }
            else if (!string.IsNullOrWhiteSpace(record.Data))
            {
                _logger.LogInformation("Holonet.Databank.Functions NewShardInQueue Skipped: User provided content (Retrieval not necessary).");
            }
            else if (string.IsNullOrWhiteSpace(record.Shard))
            {
                _logger.LogWarning("Holonet.Databank.Functions NewShardInQueue Warning: No shard provided for record ID: {RecordId}", record.Id);
            }
            else
            {
                await ProcessDataRecordDtoAsync(record);
            }
        }
        else
        {
            _logger.LogInformation("Holonet.Databank.Functions NewShardInQueue was cancelled due to UseQueueTrigger configuration setting.");
        }
    }
}