using Holonet.Databank.AppFunctions.Clients;
using Microsoft.Extensions.Logging;

namespace Holonet.Databank.AppFunctions.Services;
internal class CharacterService(ILogger<CharacterService> logger, CharacterClient characterClient)
{
    private readonly CharacterClient _characterClient = characterClient;
    private readonly ILogger _logger = logger;

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
