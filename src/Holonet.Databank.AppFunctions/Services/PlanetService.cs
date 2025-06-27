using Holonet.Databank.AppFunctions.Clients;
using Microsoft.Extensions.Logging;

namespace Holonet.Databank.AppFunctions.Services;
internal class PlanetService(ILogger<PlanetService> logger, PlanetClient planetClient)
{    
    private readonly PlanetClient _planetClient = planetClient;
    private readonly ILogger _logger = logger;

    public async Task<bool> ProcessNewDataRecord(int recordId, int? planetId, string? shard, string recordData)
    {
        bool completed = false;
        if (planetId.HasValue)
        {
            if (await _planetClient.UpdateDataRecord(recordId, planetId.Value, shard!, recordData))
            {
                _logger.LogInformation("Holonet.Databank.Functions ProcessNewDataRecord PlanetId: {PlanetId} updated successfully.", planetId);
                completed = true;
            }
            else
            {
                _logger.LogError("Holonet.Databank.Functions ProcessNewDataRecord PlanetId: {PlanetId} update failed.", planetId);
            }
        }
        else
        {
            _logger.LogWarning("Holonet.Databank.Functions ProcessNewDataRecord PlanetId is null for record ID: {RecordId}", recordId);
        }
        return completed;
    }
}
