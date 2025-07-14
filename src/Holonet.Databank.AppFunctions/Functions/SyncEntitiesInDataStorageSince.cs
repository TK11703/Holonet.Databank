using System;
using Holonet.Databank.AppFunctions.Clients;
using Holonet.Databank.AppFunctions.Configuration;
using Holonet.Databank.AppFunctions.Syncing;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Holonet.Databank.AppFunctions.Functions
{
    public class SyncEntitiesInDataStorageSince(ILoggerFactory loggerFactory, IOptions<AppSettings> appSettings, CharacterClient characterClient, HistoricalEventClient historicalEventClient, PlanetClient planetClient, SpeciesClient speciesClient)
    {
        private readonly ILogger _logger = loggerFactory.CreateLogger<SyncEntitiesInDataStorageSince>();
        private readonly ILoggerFactory _loggerFactory = loggerFactory;
        private readonly AppSettings _appSettings = appSettings.Value;
        private readonly CharacterClient _characterClient = characterClient;
        private readonly HistoricalEventClient _historicalEventClient = historicalEventClient;
        private readonly PlanetClient _planetClient = planetClient;
        private readonly SpeciesClient _speciesClient = speciesClient;

        /// <summary>
        /// Execute the sync operation for all entities in the data storage since a specified date.
        /// </summary>
        /// <param name="myTimer">The NCRONTAB format is "Seconds Minutes Hours DayofMonth Month DayOfWeek"</param>
        /// <returns></returns>
        [Function("SyncEntitiesInDataStorageSince")]
        public async Task Run([TimerTrigger("0 0 8 * * *")] TimerInfo myTimer)
        {
            DateTime executedOn = DateTime.UtcNow;
            _logger.LogInformation("Holonet.Databank.Functions SyncEntitiesInDataStorageSince executed at: {ExecutionTime}", executedOn);
            string? storConString = _appSettings.DataStorage.ConnectionString;
            if(string.IsNullOrEmpty(storConString))
            {
                _logger.LogError("Holonet.Databank.Functions SyncEntitiesInDataStorageSince error: DataStorage connection string is null or empty");
                return;
            }
            string? storContainerName = _appSettings.DataStorage.ContainerName;
            if (string.IsNullOrEmpty(storContainerName))
            {
                _logger.LogError("Holonet.Databank.Functions SyncEntitiesInDataStorageSince error: DataStorage container name is null or empty");
                return;
            }
            try
            {
                ILogger<SyncEngine> engineLogger = _loggerFactory.CreateLogger<SyncEngine>();
                SyncEngine engine = new SyncEngine(engineLogger, storConString, storContainerName, _characterClient, _historicalEventClient, _planetClient, _speciesClient);
                await engine.SyncAllEntitiesSince(executedOn.AddDays(-1));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Holonet.Databank.Functions SyncEntitiesInDataStorageSince error: {ErrorMessage}", ex.Message);
            }
            finally
            {
                _logger.LogInformation("Holonet.Databank.Functions SyncEntitiesInDataStorageSince Done!");
            }
            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation("Next timer schedule at: {NextSchedule}", myTimer.ScheduleStatus.Next);
            }
        }
    }
}
