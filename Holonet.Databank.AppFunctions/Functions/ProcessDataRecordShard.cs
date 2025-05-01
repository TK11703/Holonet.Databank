using Holonet.Databank.AppFunctions.Clients;
using Holonet.Databank.AppFunctions.HtmlHarvesting;
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
    public class ProcessDataRecordShard(ILoggerFactory loggerFactory, AIServiceClient serviceClient, CharacterClient characterClient, PlanetClient planetClient, SpeciesClient speciesClient, HistoricalEventClient historicalEventClient)
    {
        private readonly ILogger _logger = loggerFactory.CreateLogger<ProcessDataRecordShard>();
        private readonly ILoggerFactory _loggerFactory = loggerFactory;
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
                _logger.LogInformation($"Holonet.Databank.Functions ProcessDataRecordShard executed via POST HTTP in '{env}' at: {DateTime.UtcNow}");
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
                DataRecordFunctionDto? externalData = JsonSerializer.Deserialize<DataRecordFunctionDto>(requestBody);
                if (externalData == null || string.IsNullOrEmpty(externalData.Shard))
                {
                    _logger.LogError("Holonet.Databank.Functions ProcessDataRecordShard error: Invalid JSON in request body.");
                    return new ObjectResult("Invalid JSON in request body.")
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                ILogger<HtmlHarvester> harvestLogger = _loggerFactory.CreateLogger<HtmlHarvester>();
                HtmlHarvester engine = new HtmlHarvester(harvestLogger);
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
                if (externalData.CharacterId.HasValue)
                {
                    if (await _characterClient.UpdateDataRecord(externalData.Id, externalData.CharacterId.Value, externalData.Shard, summary.ResultText))
                    {
                        _logger.LogInformation("Holonet.Databank.Functions ProcessDataRecordShard CharacterId: {CharacterId} updated successfully.", externalData.CharacterId);
                    }
                    else
                    {
                        _logger.LogError("Holonet.Databank.Functions ProcessDataRecordShard CharacterId: {CharacterId} update failed.", externalData.CharacterId);
                    }
                }
                if (externalData.PlanetId.HasValue)
                {
                    if (await _planetClient.UpdateDataRecord(externalData.Id, externalData.PlanetId.Value, externalData.Shard, summary.ResultText))
                    {
                        _logger.LogInformation("Holonet.Databank.Functions ProcessDataRecordShard PlanetId: {PlanetId} updated successfully.", externalData.PlanetId);
                    }
                    else
                    {
                        _logger.LogError("Holonet.Databank.Functions ProcessDataRecordShard PlanetId: {PlanetId} update failed.", externalData.PlanetId);
                    }
                }
                if (externalData.SpeciesId.HasValue)
                {
                    if (await _speciesClient.UpdateDataRecord(externalData.Id, externalData.SpeciesId.Value, externalData.Shard, summary.ResultText))
                    {
                        _logger.LogInformation("Holonet.Databank.Functions ProcessDataRecordShard SpeciesId: {SpeciesId} updated successfully.", externalData.SpeciesId);
                    }
                    else
                    {
                        _logger.LogError("Holonet.Databank.Functions ProcessDataRecordShard SpeciesId: {SpeciesId} update failed.", externalData.SpeciesId);
                    }
                }
                if (externalData.HistoricalEventId.HasValue)
                {
                    if (await _historicalEventClient.UpdateDataRecord(externalData.Id, externalData.HistoricalEventId.Value, externalData.Shard, summary.ResultText))
                    {
                        _logger.LogInformation("Holonet.Databank.Functions ProcessDataRecordShard HistoricalEventId: {HistoricalEventId} updated successfully.", externalData.HistoricalEventId);
                    }
                    else
                    {
                        _logger.LogError("Holonet.Databank.Functions ProcessDataRecordShard HistoricalEventId: {HistoricalEventId} update failed.", externalData.HistoricalEventId);
                    }
                }
                return new OkObjectResult("Requested Sync Operation Completed!");
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
