using Holonet.Databank.AppFunctions.Clients;
using Holonet.Databank.AppFunctions.HtmlHarvesting;
using Holonet.Databank.AppFunctions.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Holonet.Databank.AppFunctions.Functions
{
    public class ProcessDataRecordShard(ILoggerFactory loggerFactory, IConfiguration configuration, AIServiceClient serviceClient, CharacterClient characterClient, PlanetClient planetClient, SpeciesClient speciesClient, HistoricalEventClient historicalEventClient)
    {
        private readonly ILogger _logger = loggerFactory.CreateLogger<ProcessDataRecordShard>();
        private readonly ILoggerFactory _loggerFactory = loggerFactory;
        private readonly IConfiguration _configuration = configuration;
        private readonly AIServiceClient _serviceClient = serviceClient;
        private readonly CharacterClient _characterClient = characterClient;
        private readonly PlanetClient _planetClient = planetClient;
        private readonly SpeciesClient _speciesClient = speciesClient;
        private readonly HistoricalEventClient _historicalEventClient = historicalEventClient;

        [Function("ProcessDataRecordShard")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            DateTime executedOn = DateTime.UtcNow;
            string? requestBody = null;
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown";
            _logger.LogInformation("Holonet.Databank.Functions ProcessDataRecordShard executed at: {ExecutionTime}", executedOn);
            if (req.Method == HttpMethods.Post)
            {
                _logger.LogInformation("Holonet.Databank.Functions ProcessDataRecordShard executed via POST HTTP in '{Environment}' at: {ExecutionTime}", env, DateTime.UtcNow);
                requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                if (string.IsNullOrEmpty(requestBody))
                {
                    _logger.LogError("Holonet.Databank.Functions ProcessDataRecordShard error: Missing JSON in request body.");
                    return new ObjectResult("Missing JSON in request body.")
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
            }
            else
            {
                throw new ArgumentException("Unsupported HTTP method.");
            }
            try
            {
                DataRecordFunctionDto? externalData = JsonSerializer.Deserialize<DataRecordFunctionDto>(requestBody, new JsonSerializerOptions() { PropertyNameCaseInsensitive=true});
                if (externalData == null || string.IsNullOrEmpty(externalData.Shard))
                {
                    _logger.LogError("Holonet.Databank.Functions ProcessDataRecordShard error: Invalid JSON in request body.");
                    return new ObjectResult("Invalid JSON in request body.")
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                ILogger<HtmlHarvester> harvestLogger = _loggerFactory.CreateLogger<HtmlHarvester>();
                HtmlHarvester engine = new HtmlHarvester(harvestLogger, _configuration);
                string harvestedHtml = await engine.HarvestHtml(externalData.Shard);
                if (string.IsNullOrEmpty(harvestedHtml))
                {
                    _logger.LogError("Holonet.Databank.Functions ProcessDataRecordShard error: Unable to harvest HTML.");
                    return new ObjectResult("Unable to harvest HTML.")
                    {
                        StatusCode = StatusCodes.Status500InternalServerError
                    };
                }
                TextSummaryResult? summary = await _serviceClient.ExecuteTextSummarization(harvestedHtml);
                if (summary == null || string.IsNullOrEmpty(summary.ResultText))
                {
                    _logger.LogError("Holonet.Databank.Functions ProcessDataRecordShard error: Unable to summarize text.");
                    return new ObjectResult("Unable to summarize text.")
                    {
                        StatusCode = StatusCodes.Status500InternalServerError
                    };
                }
                
                bool errorOccurred = false;
                if (externalData.CharacterId.HasValue)
                {
                    ILogger<CharacterService> serviceLogger = _loggerFactory.CreateLogger<CharacterService>();
                    CharacterService characterService = new CharacterService(serviceLogger, _characterClient);
                    if (await characterService.ProcessNewDataRecord(externalData.Id, externalData.CharacterId, externalData.Shard, summary.ResultText))
                    {
                        _logger.LogInformation("Holonet.Databank.Functions ProcessDataRecordShard CharacterId: {CharacterId} updated successfully.", externalData.CharacterId);
                    }
                    else
                    {
                        errorOccurred = true;
                        _logger.LogError("Holonet.Databank.Functions ProcessDataRecordShard CharacterId: {CharacterId} update failed.", externalData.CharacterId);
                    }
                }
                if (externalData.PlanetId.HasValue)
                {
                    ILogger<PlanetService> serviceLogger = _loggerFactory.CreateLogger<PlanetService>();
                    PlanetService planetService = new PlanetService(serviceLogger, _planetClient);
                    if (await planetService.ProcessNewDataRecord(externalData.Id, externalData.PlanetId, externalData.Shard, summary.ResultText))
                    {
                        _logger.LogInformation("Holonet.Databank.Functions ProcessDataRecordShard PlanetId: {PlanetId} updated successfully.", externalData.PlanetId);
                    }
                    else
                    {
                        errorOccurred = true;
                        _logger.LogError("Holonet.Databank.Functions ProcessDataRecordShard PlanetId: {PlanetId} update failed.", externalData.PlanetId);
                    }
                }
                if (externalData.SpeciesId.HasValue)
                {
                    ILogger<SpeciesService> serviceLogger = _loggerFactory.CreateLogger<SpeciesService>();
                    SpeciesService speciesService = new SpeciesService(serviceLogger, _speciesClient);
                    if (await speciesService.ProcessNewDataRecord(externalData.Id, externalData.SpeciesId, externalData.Shard, summary.ResultText))
                    {
                        _logger.LogInformation("Holonet.Databank.Functions ProcessDataRecordShard SpeciesId: {SpeciesId} updated successfully.", externalData.SpeciesId);
                    }
                    else
                    {
                        errorOccurred = true;
                        _logger.LogError("Holonet.Databank.Functions ProcessDataRecordShard SpeciesId: {SpeciesId} update failed.", externalData.SpeciesId);
                    }
                }
                if (externalData.HistoricalEventId.HasValue)
                {
                    ILogger<HistoricalEventService> serviceLogger = _loggerFactory.CreateLogger<HistoricalEventService>();
                    HistoricalEventService historicalEventService = new HistoricalEventService(serviceLogger, _historicalEventClient);
                    if (await historicalEventService.ProcessNewDataRecord(externalData.Id, externalData.HistoricalEventId, externalData.Shard, summary.ResultText))
                    {
                        _logger.LogInformation("Holonet.Databank.Functions ProcessDataRecordShard HistoricalEventId: {HistoricalEventId} updated successfully.", externalData.HistoricalEventId);
                    }
                    else
                    {
                        errorOccurred = true;
                        _logger.LogError("Holonet.Databank.Functions ProcessDataRecordShard HistoricalEventId: {HistoricalEventId} update failed.", externalData.HistoricalEventId);
                    }
                }
                if(errorOccurred)
                {
                    return new ObjectResult("Holonet.Databank.Functions ProcessDataRecordShard failed.")
                    {
                        StatusCode = StatusCodes.Status500InternalServerError
                    };
                }
                return new OkObjectResult("Holonet.Databank.Functions ProcessDataRecordShard Completed!");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Holonet.Databank.Functions ProcessDataRecordShard error: Invalid JSON in request body.");
                return new ObjectResult("Invalid JSON in request body.")
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
        }
    }
}
