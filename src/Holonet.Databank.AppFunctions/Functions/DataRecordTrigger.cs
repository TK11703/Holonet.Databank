using Holonet.Databank.AppFunctions.Clients;
using Holonet.Databank.AppFunctions.Configuration;
using Holonet.Databank.Core.Dtos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Holonet.Databank.AppFunctions.Functions;

public class DataRecordTrigger(ILoggerFactory loggerFactory, IOptions<AppSettings> appSettings, AIServiceClient serviceClient, CharacterClient characterClient, PlanetClient planetClient, SpeciesClient speciesClient, HistoricalEventClient historicalEventClient)
    : DataRecordDtoProcessor(loggerFactory, appSettings, serviceClient, characterClient, planetClient, speciesClient, historicalEventClient)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<DataRecordTrigger>();
    private readonly AppSettings _settings = appSettings.Value;

    // Visit https://aka.ms/sqltrigger to learn how to use this trigger binding
    [Function("datarecordtrigger")]
    public async Task Run([SqlTrigger("[dbo].[DataRecords]", "DefaultSQLDBConnection")] IReadOnlyList<SqlChange<DataRecordFunctionDto>> changes, FunctionContext context)
    {
        DateTime executedOn = DateTime.UtcNow;
        _logger.LogInformation("Holonet.Databank.Functions DataRecordTrigger was triggered at: {ExecutionTime}.", executedOn);

        if (_settings.UseSqlTrigger)
        {
            var inserts = changes.Where(c => c.Operation == SqlChangeOperation.Insert).ToList();
            _logger.LogInformation("Insert operations detected: {TotalInserts}", inserts.Count);

            foreach (var change in inserts)
            {
                _logger.LogInformation("Inserted record: {RecordId}", change.Item.Id);

                if (change.Item.Id.Equals(0))
                {
                    _logger.LogError("Holonet.Databank.Functions DataRecordTrigger error: Invalid data record - no item ID.");
                }
                else if (!string.IsNullOrWhiteSpace(change.Item.Data))
                {
                    _logger.LogInformation("Holonet.Databank.Functions DataRecordTrigger Skipped: User provided content (Retrieval not necessary).");
                }
                else if (!string.IsNullOrWhiteSpace(change.Item.Shard))
                {
                    await ProcessDataRecordDtoAsync(change.Item);
                }
                else
                {
                    _logger.LogWarning("Holonet.Databank.Functions DataRecordTrigger Warning: No shard provided for record ID: {RecordId}", change.Item.Id);
                }
            }
        }
        else
        {
            _logger.LogInformation("Holonet.Databank.Functions DataRecordTrigger was cancelled due to UseSqlTrigger configuration setting.");
        }
    }
}