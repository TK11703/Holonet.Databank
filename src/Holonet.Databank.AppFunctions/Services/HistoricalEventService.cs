using Holonet.Databank.AppFunctions.Clients;
using Microsoft.Extensions.Logging;

namespace Holonet.Databank.AppFunctions.Services;
internal class HistoricalEventService(ILogger<HistoricalEventService> logger, HistoricalEventClient historicalEventClient)
{
    private readonly HistoricalEventClient _historicalEventClient = historicalEventClient;
    private readonly ILogger _logger = logger;

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
