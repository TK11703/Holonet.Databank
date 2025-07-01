using Holonet.Databank.AppFunctions.Clients;
using Holonet.Databank.AppFunctions.HtmlHarvesting;
using Holonet.Databank.AppFunctions.Services;
using Holonet.Databank.Core.Dtos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Holonet.Databank.AppFunctions.Functions;

public class DataRecordTrigger(ILoggerFactory loggerFactory, IConfiguration configuration, AIServiceClient serviceClient, CharacterClient characterClient, PlanetClient planetClient, SpeciesClient speciesClient, HistoricalEventClient historicalEventClient)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<DataRecordTrigger>();
    private readonly IConfiguration _configuration = configuration;
    private readonly ILoggerFactory _loggerFactory = loggerFactory;
    private readonly AIServiceClient _serviceClient = serviceClient;
    private readonly CharacterClient _characterClient = characterClient;
    private readonly PlanetClient _planetClient = planetClient;
    private readonly SpeciesClient _speciesClient = speciesClient;
    private readonly HistoricalEventClient _historicalEventClient = historicalEventClient;

    // Visit https://aka.ms/sqltrigger to learn how to use this trigger binding
    [Function("DataRecordTrigger")]
    public async Task Run([SqlTrigger("[dbo].[DataRecords]", "ConnectionStrings:DefaultConnection")] IReadOnlyList<SqlChange<DataRecordDto>> changes, FunctionContext context)
    {
        DateTime executedOn = DateTime.UtcNow;
        _logger.LogInformation("Holonet.Databank.Functions DataRecordTrigger executed at: {ExecutionTime}", executedOn);

        foreach (var change in changes)
        {
            if (change.Operation == SqlChangeOperation.Insert)
            {
                _logger.LogInformation("New Data Record Inserted: {RecordId}", change.Item.Id);

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
                    IEnumerable<string> harvestedHtmlChunks;
                    try
                    {
                        ILogger<HtmlHarvester> harvestLogger = _loggerFactory.CreateLogger<HtmlHarvester>();
                        HtmlHarvester engine = new HtmlHarvester(harvestLogger, _configuration);
                        harvestedHtmlChunks = await engine.HarvestHtml(change.Item.Shard);
                        if (harvestedHtmlChunks.Count().Equals(0))
                        {
                            _logger.LogError("Holonet.Databank.Functions DataRecordTrigger error: Unable to harvest HTML.");
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Holonet.Databank.Functions DataRecordTrigger error: Exception occurred while harvesting HTML.");
                        throw;
                    }
                    string summary = string.Empty;
                    try
                    {
                        summary = await _serviceClient.ExecuteTextSummarization(harvestedHtmlChunks);
                        if (string.IsNullOrWhiteSpace(summary))
                        {
                            _logger.LogError("Holonet.Databank.Functions DataRecordTrigger error: Unable to summarize text.");
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Holonet.Databank.Functions DataRecordTrigger error: Exception occurred while summarizing text.");
                        throw;
                    }

                    if (change.Item.CharacterId.HasValue)
                    {
                        ILogger<CharacterService> serviceLogger = _loggerFactory.CreateLogger<CharacterService>();
                        CharacterService characterService = new CharacterService(serviceLogger, _characterClient);
                        if (await characterService.ProcessNewDataRecord(change.Item.Id, change.Item.CharacterId, change.Item.Shard, summary))
                        {
                            _logger.LogInformation("Holonet.Databank.Functions DataRecordTrigger CharacterId: {CharacterId} updated successfully.", change.Item.CharacterId);
                        }
                        else
                        {
                            _logger.LogError("Holonet.Databank.Functions DataRecordTrigger CharacterId: {CharacterId} update failed.", change.Item.CharacterId);
                        }
                    }
                    if (change.Item.PlanetId.HasValue)
                    {
                        ILogger<PlanetService> serviceLogger = _loggerFactory.CreateLogger<PlanetService>();
                        PlanetService planetService = new PlanetService(serviceLogger, _planetClient);
                        if (await planetService.ProcessNewDataRecord(change.Item.Id, change.Item.PlanetId, change.Item.Shard, summary))
                        {
                            _logger.LogInformation("Holonet.Databank.Functions DataRecordTrigger PlanetId: {PlanetId} updated successfully.", change.Item.PlanetId);
                        }
                        else
                        {
                            _logger.LogError("Holonet.Databank.Functions DataRecordTrigger PlanetId: {PlanetId} update failed.", change.Item.PlanetId);
                        }
                    }
                    if (change.Item.SpeciesId.HasValue)
                    {
                        ILogger<SpeciesService> serviceLogger = _loggerFactory.CreateLogger<SpeciesService>();
                        SpeciesService speciesService = new SpeciesService(serviceLogger, _speciesClient);
                        if (await speciesService.ProcessNewDataRecord(change.Item.Id, change.Item.SpeciesId, change.Item.Shard, summary))
                        {
                            _logger.LogInformation("Holonet.Databank.Functions DataRecordTrigger SpeciesId: {SpeciesId} updated successfully.", change.Item.SpeciesId);
                        }
                        else
                        {
                            _logger.LogError("Holonet.Databank.Functions DataRecordTrigger SpeciesId: {SpeciesId} update failed.", change.Item.SpeciesId);
                        }
                    }
                    if (change.Item.HistoricalEventId.HasValue)
                    {
                        ILogger<HistoricalEventService> serviceLogger = _loggerFactory.CreateLogger<HistoricalEventService>();
                        HistoricalEventService historicalEventService = new HistoricalEventService(serviceLogger, _historicalEventClient);
                        if (await historicalEventService.ProcessNewDataRecord(change.Item.Id, change.Item.HistoricalEventId, change.Item.Shard, summary))
                        {
                            _logger.LogInformation("Holonet.Databank.Functions DataRecordTrigger HistoricalEventId: {HistoricalEventId} updated successfully.", change.Item.HistoricalEventId);
                        }
                        else
                        {
                            _logger.LogError("Holonet.Databank.Functions DataRecordTrigger HistoricalEventId: {HistoricalEventId} update failed.", change.Item.HistoricalEventId);
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("Holonet.Databank.Functions DataRecordTrigger Warning: No shard provided for record ID: {RecordId}", change.Item.Id);
                }
            }
        }
    }
}