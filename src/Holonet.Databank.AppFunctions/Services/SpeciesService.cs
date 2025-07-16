using Holonet.Databank.AppFunctions.Clients;
using Microsoft.Extensions.Logging;

namespace Holonet.Databank.AppFunctions.Services;
internal class SpeciesService(ILogger<SpeciesService> logger, SpeciesClient speciesClient)
{
    private readonly SpeciesClient _speciesClient = speciesClient;
    private readonly ILogger _logger = logger;

    public async Task<bool> UpdateDataRecordForProcessing(int recordId, int? speciesId, string? shard, string? errorMessage)
    {
        bool completed = false;
        if (speciesId.HasValue)
        {
            if (string.IsNullOrEmpty(errorMessage))
            {
                if (await _speciesClient.UpdateDataRecordForProcessing(recordId, speciesId.Value, shard!))
                {
                    _logger.LogInformation("Holonet.Databank.Functions UpdateDataRecordForProcessing SpeciesId: {SpeciesId} updated successfully.", speciesId);
                    completed = true;
                }
                else
                {
                    _logger.LogError("Holonet.Databank.Functions UpdateDataRecordForProcessing SpeciesId: {SpeciesId} update failed.", speciesId);
                }
            }
            else
            {
                if (await _speciesClient.UpdateDataRecordForProcessingError(recordId, speciesId.Value, shard!, errorMessage!))
                {
                    _logger.LogInformation("Holonet.Databank.Functions UpdateDataRecordForProcessingError SpeciesId: {SpeciesId} updated successfully.", speciesId);
                    completed = true;
                }
                else
                {
                    _logger.LogError("Holonet.Databank.Functions UpdateDataRecordForProcessingError SpeciesId: {SpeciesId} update failed.", speciesId);
                }
            }
        }
        else
        {
            _logger.LogWarning("Holonet.Databank.Functions UpdateDataRecordForProcessing SpeciesId is null for record ID: {RecordId}", recordId);
        }
        return completed;
    }

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
