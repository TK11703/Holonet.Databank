using Holonet.Databank.AppFunctions.Clients;
using Holonet.Databank.AppFunctions.Configuration;
using Holonet.Databank.AppFunctions.Syncing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Holonet.Databank.AppFunctions.Functions
{
    public class SyncAllEntitiesInDataStorage(ILoggerFactory loggerFactory, IOptions<AppSettings> appSettings, CharacterClient characterClient, HistoricalEventClient historicalEventClient, PlanetClient planetClient, SpeciesClient speciesClient)
    {
        private readonly ILogger _logger = loggerFactory.CreateLogger<SyncAllEntitiesInDataStorage>();
        private readonly ILoggerFactory _loggerFactory = loggerFactory;
        private readonly AppSettings _appSettings = appSettings.Value;
        private readonly CharacterClient _characterClient = characterClient;
        private readonly HistoricalEventClient _historicalEventClient = historicalEventClient;
        private readonly PlanetClient _planetClient = planetClient;
        private readonly SpeciesClient _speciesClient = speciesClient;

        [Function("SyncAllEntitiesInDataStorage")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            DateTime executedOn = DateTime.UtcNow;
            _logger.LogInformation("Holonet.Databank.Functions SyncAllEntitiesInDataStorage executed at: {ExecutionTime}", executedOn);
            string? storConString = _appSettings.DataStorage.ConnectionString;
            if (string.IsNullOrEmpty(storConString))
            {
                _logger.LogError("Holonet.Databank.Functions SyncAllEntitiesInDataStorage error: DataStorage connection string is null or empty");
                return new ObjectResult(new { error = "An internal server error occurred." }) { StatusCode = 500 };
            }
            string? storContainerName = _appSettings.DataStorage.ContainerName;
            if (string.IsNullOrEmpty(storContainerName))
            {
                _logger.LogError("Holonet.Databank.Functions SyncAllEntitiesInDataStorage error: DataStorage container name is null or empty");
                return new ObjectResult(new { error = "An internal server error occurred." }) { StatusCode = 500 };
            }
            try
            {
                ILogger<SyncEngine> engineLogger = _loggerFactory.CreateLogger<SyncEngine>();
                SyncEngine engine = new SyncEngine(engineLogger, storConString, storContainerName, _characterClient, _historicalEventClient, _planetClient, _speciesClient);
                await engine.SyncAllEntities();
                return new OkObjectResult("Requested Sync Operation Completed!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Holonet.Databank.Functions SyncAllEntitiesInDataStorage error: {ErrorMessage}", ex.Message);
                return new ObjectResult(new { error = "An internal server error occurred." }) { StatusCode = 500 };
            }
            finally
            {
                _logger.LogInformation("Holonet.Databank.Functions SyncAllEntitiesInDataStorage Done!");
            }
        }
    }
}
