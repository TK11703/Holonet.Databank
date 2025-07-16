using Holonet.Databank.AppFunctions.Clients;
using Microsoft.Extensions.Logging;

namespace Holonet.Databank.AppFunctions.Services;
internal class CharacterService(ILogger<CharacterService> logger, CharacterClient characterClient)
{
    private readonly CharacterClient _characterClient = characterClient;
    private readonly ILogger _logger = logger;

    public async Task<bool> UpdateDataRecordForProcessing(int recordId, int? characterId, string? shard, string? errorMessage)
    {
        bool completed = false;
        if (characterId.HasValue)
        {
            if(string.IsNullOrEmpty(errorMessage))
            {
                if (await _characterClient.UpdateDataRecordForProcessing(recordId, characterId.Value, shard!))
                {
                    _logger.LogInformation("Holonet.Databank.Functions UpdateDataRecordForProcessing CharacterId: {CharacterId} updated successfully.", characterId);
                    completed = true;
                }
                else
                {
                    _logger.LogError("Holonet.Databank.Functions UpdateDataRecordForProcessing CharacterId: {CharacterId} update failed.", characterId.Value);
                }
            }
            else
            {
                if (await _characterClient.UpdateDataRecordForProcessingError(recordId, characterId.Value, shard!, errorMessage!))
                {
                    _logger.LogInformation("Holonet.Databank.Functions UpdateDataRecordForProcessingError CharacterId: {CharacterId} updated successfully.", characterId);
                    completed = true;
                }
                else
                {
                    _logger.LogError("Holonet.Databank.Functions UpdateDataRecordForProcessingError CharacterId: {CharacterId} update failed.", characterId.Value);
                }
            }
                
        }
        else
        {
            _logger.LogWarning("Holonet.Databank.Functions UpdateDataRecordForProcessing CharacterId is null for record ID: {RecordId}", recordId);
        }
        return completed;
    }

    public async Task<bool> ProcessNewDataRecord(int recordId, int? characterId, string? shard, string recordData)
    {
        bool completed = false;
        if (characterId.HasValue)
        {
            if (await _characterClient.UpdateDataRecord(recordId, characterId.Value, shard!, recordData))
            {
                _logger.LogInformation("Holonet.Databank.Functions ProcessNewDataRecord CharacterId: {CharacterId} updated successfully.", characterId);
                completed = true;
            }
            else
            {
                _logger.LogError("Holonet.Databank.Functions ProcessNewDataRecord CharacterId: {CharacterId} update failed.", characterId.Value);
            }
        }
        else
        {
            _logger.LogWarning("Holonet.Databank.Functions ProcessNewDataRecord CharacterId is null for record ID: {RecordId}", recordId);
        }
        return completed;
    }
}
