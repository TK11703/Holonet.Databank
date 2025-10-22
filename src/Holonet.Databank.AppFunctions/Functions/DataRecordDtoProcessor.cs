using Holonet.Databank.AppFunctions.Clients;
using Holonet.Databank.AppFunctions.Configuration;
using Holonet.Databank.AppFunctions.HtmlHarvesting;
using Holonet.Databank.AppFunctions.Services;
using Holonet.Databank.Core.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Holonet.Databank.AppFunctions.Functions
{
    public class DataRecordDtoProcessor(ILoggerFactory loggerFactory, IOptions<AppSettings> appSettings, AIServiceClient serviceClient, CharacterClient characterClient, PlanetClient planetClient, SpeciesClient speciesClient, HistoricalEventClient historicalEventClient)
    {
        private readonly ILogger _logger = loggerFactory.CreateLogger<DataRecordDtoProcessor>();
        private readonly AppSettings _settings = appSettings.Value;
        private readonly ILoggerFactory _loggerFactory = loggerFactory;
        private readonly AIServiceClient _serviceClient = serviceClient;
        private readonly CharacterClient _characterClient = characterClient;
        private readonly PlanetClient _planetClient = planetClient;
        private readonly SpeciesClient _speciesClient = speciesClient;
        private readonly HistoricalEventClient _historicalEventClient = historicalEventClient;

        protected async Task ProcessDataRecordDtoAsync(DataRecordFunctionDto record)
        {
            bool recordUpdatedForProcessing = await UpdateDataRecordForProcessing(record);
            if (!recordUpdatedForProcessing)
            {
                _logger.LogError("Holonet.Databank.Functions ProcessDataRecordDto error: Unable to update status of data record.");
            }

            IEnumerable<string> harvestedHtmlChunks;
            try
            {
                ILogger<HtmlHarvester> harvestLogger = _loggerFactory.CreateLogger<HtmlHarvester>();
                HtmlHarvester engine = new HtmlHarvester(harvestLogger, _settings);
                harvestedHtmlChunks = await engine.HarvestHtml(record.Shard!);
                if (harvestedHtmlChunks.Count().Equals(0))
                {
                    await UpdateDataRecordForProcessing(record, MessageConstants.HtmlHarvesterNoData);
                    _logger.LogError("Holonet.Databank.Functions ProcessDataRecordDto error: Unable to harvest HTML.");
                }
            }
            catch (Exception ex)
            {
                await UpdateDataRecordForProcessing(record, MessageConstants.HtmlHarvesterError);
                _logger.LogError(ex, "Holonet.Databank.Functions ProcessDataRecordDto error: Exception occurred while harvesting HTML.");
                throw;
            }
            string summary = string.Empty;
            try
            {
                summary = await _serviceClient.ExecuteTextSummarization(harvestedHtmlChunks);
                if (string.IsNullOrWhiteSpace(summary))
                {
                    await UpdateDataRecordForProcessing(record, MessageConstants.TextSummarizationNoData);
                    _logger.LogError("Holonet.Databank.Functions ProcessDataRecordDto error: Unable to summarize text.");
                }
            }
            catch (Exception ex)
            {
                await UpdateDataRecordForProcessing(record, MessageConstants.TextSummarizationError);
                _logger.LogError(ex, "Holonet.Databank.Functions ProcessDataRecordDto error: Exception occurred while summarizing text.");
                throw;
            }

            bool processSuccessful = await ProcessNewDataRecord(record, summary);
            if(!processSuccessful)
            {
                _logger.LogError("Holonet.Databank.Functions ProcessDataRecordDto error: ProcessDataRecordShard failed.");
            }
        }

        protected async Task<bool> UpdateDataRecordForProcessing(DataRecordFunctionDto record, string? errorMessage = null)
        {
            bool completed = false;
            if (record.Id.Equals(0))
            {
                _logger.LogError("Holonet.Databank.Functions ProcessDataRecordDto error: Invalid data record - no item ID.");
                return completed;
            }
            if (record.CharacterId.HasValue)
            {
                ILogger<CharacterService> serviceLogger = _loggerFactory.CreateLogger<CharacterService>();
                CharacterService characterService = new CharacterService(serviceLogger, _characterClient);
                if (await characterService.UpdateDataRecordForProcessing(record.Id, record.CharacterId, record.Shard, errorMessage))
                {
                    _logger.LogInformation("Holonet.Databank.Functions ProcessDataRecordDto CharacterId: {CharacterId} updated successfully.", record.CharacterId);
                    return true;
                }
                else
                {
                    _logger.LogError("Holonet.Databank.Functions ProcessDataRecordDto CharacterId: {CharacterId} update failed.", record.CharacterId);
                }
            }
            else if (record.PlanetId.HasValue)
            {
                ILogger<PlanetService> serviceLogger = _loggerFactory.CreateLogger<PlanetService>();
                PlanetService planetService = new PlanetService(serviceLogger, _planetClient);
                if (await planetService.UpdateDataRecordForProcessing(record.Id, record.PlanetId, record.Shard, errorMessage))
                {
                    _logger.LogInformation("Holonet.Databank.Functions ProcessDataRecordDto PlanetId: {PlanetId} updated successfully.", record.PlanetId);
                    return true;
                }
                else
                {
                    _logger.LogError("Holonet.Databank.Functions ProcessDataRecordDto PlanetId: {PlanetId} update failed.", record.PlanetId);
                }
            }
            else if (record.SpeciesId.HasValue)
            {
                ILogger<SpeciesService> serviceLogger = _loggerFactory.CreateLogger<SpeciesService>();
                SpeciesService speciesService = new SpeciesService(serviceLogger, _speciesClient);
                if (await speciesService.UpdateDataRecordForProcessing(record.Id, record.SpeciesId, record.Shard, errorMessage))
                {
                    _logger.LogInformation("Holonet.Databank.Functions ProcessDataRecordDto SpeciesId: {SpeciesId} updated successfully.", record.SpeciesId);
                    return true;
                }
                else
                {
                    _logger.LogError("Holonet.Databank.Functions ProcessDataRecordDto SpeciesId: {SpeciesId} update failed.", record.SpeciesId);
                }
            }
            else if (record.HistoricalEventId.HasValue)
            {
                ILogger<HistoricalEventService> serviceLogger = _loggerFactory.CreateLogger<HistoricalEventService>();
                HistoricalEventService historicalEventService = new HistoricalEventService(serviceLogger, _historicalEventClient);
                if (await historicalEventService.UpdateDataRecordForProcessing(record.Id, record.HistoricalEventId, record.Shard, errorMessage))
                {
                    _logger.LogInformation("Holonet.Databank.Functions ProcessDataRecordDto HistoricalEventId: {HistoricalEventId} updated successfully.", record.HistoricalEventId);
                    return true;
                }
                else
                {
                    _logger.LogError("Holonet.Databank.Functions ProcessDataRecordDto HistoricalEventId: {HistoricalEventId} update failed.", record.HistoricalEventId);
                }
            }
            else
            {
                _logger.LogWarning("Holonet.Databank.Functions ProcessDataRecordDto Warning: No valid ID provided for record ID: {RecordId}", record.Id);
            }
            return completed;
        }

        protected async Task<bool> ProcessNewDataRecord(DataRecordFunctionDto record, string summary)
        {
            if (record.CharacterId.HasValue)
            {
                ILogger<CharacterService> serviceLogger = _loggerFactory.CreateLogger<CharacterService>();
                CharacterService characterService = new CharacterService(serviceLogger, _characterClient);
                if (await characterService.ProcessNewDataRecord(record.Id, record.CharacterId, record.Shard, summary))
                {
                    _logger.LogInformation("Holonet.Databank.Functions ProcessDataRecordDto CharacterId: {CharacterId} updated successfully.", record.CharacterId);
                    return true;
                }
                else
                {
                    _logger.LogError("Holonet.Databank.Functions ProcessDataRecordDto CharacterId: {CharacterId} update failed.", record.CharacterId);
                }
            }
            if (record.PlanetId.HasValue)
            {
                ILogger<PlanetService> serviceLogger = _loggerFactory.CreateLogger<PlanetService>();
                PlanetService planetService = new PlanetService(serviceLogger, _planetClient);
                if (await planetService.ProcessNewDataRecord(record.Id, record.PlanetId, record.Shard, summary))
                {
                    _logger.LogInformation("Holonet.Databank.Functions ProcessDataRecordDto PlanetId: {PlanetId} updated successfully.", record.PlanetId);
                    return true;
                }
                else
                {
                    _logger.LogError("Holonet.Databank.Functions ProcessDataRecordDto PlanetId: {PlanetId} update failed.", record.PlanetId);
                }
            }
            if (record.SpeciesId.HasValue)
            {
                ILogger<SpeciesService> serviceLogger = _loggerFactory.CreateLogger<SpeciesService>();
                SpeciesService speciesService = new SpeciesService(serviceLogger, _speciesClient);
                if (await speciesService.ProcessNewDataRecord(record.Id, record.SpeciesId, record.Shard, summary))
                {
                    _logger.LogInformation("Holonet.Databank.Functions ProcessDataRecordDto SpeciesId: {SpeciesId} updated successfully.", record.SpeciesId);
                    return true;
                }
                else
                {
                    _logger.LogError("Holonet.Databank.Functions ProcessDataRecordDto SpeciesId: {SpeciesId} update failed.", record.SpeciesId);
                }
            }
            if (record.HistoricalEventId.HasValue)
            {
                ILogger<HistoricalEventService> serviceLogger = _loggerFactory.CreateLogger<HistoricalEventService>();
                HistoricalEventService historicalEventService = new HistoricalEventService(serviceLogger, _historicalEventClient);
                if (await historicalEventService.ProcessNewDataRecord(record.Id, record.HistoricalEventId, record.Shard, summary))
                {
                    _logger.LogInformation("Holonet.Databank.Functions ProcessDataRecordDto HistoricalEventId: {HistoricalEventId} updated successfully.", record.HistoricalEventId);
                    return true;
                }
                else
                {
                    _logger.LogError("Holonet.Databank.Functions ProcessDataRecordDto HistoricalEventId: {HistoricalEventId} update failed.", record.HistoricalEventId);
                }
            }
            return false;
        }
    }
}
