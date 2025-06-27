using Holonet.Databank.AppFunctions.Clients;
using Microsoft.Extensions.Logging;

namespace Holonet.Databank.AppFunctions.Services;
internal class SpeciesService(ILogger<SpeciesService> logger, SpeciesClient speciesClient)
{
    private readonly SpeciesClient _speciesClient = speciesClient;
    private readonly ILogger _logger = logger;

    public async Task<bool> ProcessNewDataRecord(int recordId, int? speciesId, string? shard, string recordData)
    {
        bool completed = false;
        if (speciesId.HasValue)
        {
            if (await _speciesClient.UpdateDataRecord(recordId, speciesId.Value, shard!, recordData))
            {
                _logger.LogInformation("Holonet.Databank.Functions ProcessNewDataRecord SpeciesId: {SpeciesId} updated successfully.", speciesId);
                completed = true;
            }
            else
            {
                _logger.LogError("Holonet.Databank.Functions ProcessNewDataRecord SpeciesId: {SpeciesId} update failed.", speciesId);
            }
        }
        else
        {
            _logger.LogWarning("Holonet.Databank.Functions ProcessNewDataRecord SpeciesId is null for record ID: {RecordId}", recordId);
        }
        return completed;
    }
}
