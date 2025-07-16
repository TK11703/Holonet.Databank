using Holonet.Databank.AppFunctions.Clients;
using Microsoft.Extensions.Logging;

namespace Holonet.Databank.AppFunctions.Services;
internal class HistoricalEventService(ILogger<HistoricalEventService> logger, HistoricalEventClient historicalEventClient)
{
    private readonly HistoricalEventClient _historicalEventClient = historicalEventClient;
    private readonly ILogger _logger = logger;

    public async Task<bool> UpdateDataRecordForProcessing(int recordId, int? historicalEventId, string? shard, string? errorMessage)
    {
        bool completed = false;
        if (historicalEventId.HasValue)
        {
            if (string.IsNullOrEmpty(errorMessage))
            {
                if (await _historicalEventClient.UpdateDataRecordForProcessing(recordId, historicalEventId.Value, shard!))
                {
                    _logger.LogInformation("Holonet.Databank.Functions UpdateDataRecordForProcessing HistoricalEventId: {HistoricalEventId} updated successfully.", historicalEventId);
                    completed = true;
                }
                else
                {
                    _logger.LogError("Holonet.Databank.Functions UpdateDataRecordForProcessing HistoricalEventId: {HistoricalEventId} update failed.", historicalEventId);
                }
            }
            else
            {
                if (await _historicalEventClient.UpdateDataRecordForProcessingError(recordId, historicalEventId.Value, shard!, errorMessage!))
                {
                    _logger.LogInformation("Holonet.Databank.Functions UpdateDataRecordForProcessingError HistoricalEventId: {HistoricalEventId} updated successfully.", historicalEventId);
                    completed = true;
                }
                else
                {
                    _logger.LogError("Holonet.Databank.Functions UpdateDataRecordForProcessingError HistoricalEventId: {HistoricalEventId} update failed.", historicalEventId);
                }
            }
        }
        else
        {
            _logger.LogWarning("Holonet.Databank.Functions UpdateDataRecordForProcessing HistoricalEventId is null for record ID: {RecordId}", recordId);
        }
        return completed;
    }

    public async Task<bool> ProcessNewDataRecord(int recordId, int? historicalEventId, string? shard, string recordData)
    {
        bool completed = false;
        if (historicalEventId.HasValue)
        {
            if (await _historicalEventClient.UpdateDataRecord(recordId, historicalEventId.Value, shard!, recordData))
            {
                _logger.LogInformation("Holonet.Databank.Functions ProcessNewDataRecord HistoricalEventId: {HistoricalEventId} updated successfully.", historicalEventId);
                completed = true;
            }
            else
            {
                _logger.LogError("Holonet.Databank.Functions ProcessNewDataRecord HistoricalEventId: {HistoricalEventId} update failed.", historicalEventId);
            }
        }
        else
        {
            _logger.LogWarning("Holonet.Databank.Functions ProcessNewDataRecord HistoricalEventId is null for record ID: {RecordId}", recordId);
        }
        return completed;
    }
}
