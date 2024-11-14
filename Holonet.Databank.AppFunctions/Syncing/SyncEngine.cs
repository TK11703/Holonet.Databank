using Holonet.Databank.AppFunctions.Clients;
using Holonet.Databank.Core.Dtos;
using Microsoft.Extensions.Logging;

namespace Holonet.Databank.AppFunctions.Syncing;
internal class SyncEngine(ILogger<SyncEngine> logger, string connectionstring, string containerName, CharacterClient characterClient, HistoricalEventClient historicalEventClient, PlanetClient planetClient, SpeciesClient speciesClient)
{
    private readonly ILogger<SyncEngine> _logger = logger;
    private readonly CharacterClient _characterClient = characterClient;
    private readonly HistoricalEventClient _historicalEventClient = historicalEventClient;
    private readonly PlanetClient _planetClient = planetClient;
    private readonly SpeciesClient _speciesClient = speciesClient;
    private readonly BlobStorageService blobStorageService = new BlobStorageService(connectionstring, containerName);

    public async Task<bool> SyncAllEntities()
    {
        var completed = false;
        try
        {
            await SyncAllCharacters();
            await SyncAllHistoricalEvents();
            await SyncAllPlanets();
            await SyncAllSpecies();
            completed = true;
        }
        catch(Exception ex)
        {
            LogMessage($"Error: {ex.Message}", LogLevel.Error);
        }
        return completed;
    }

    public async Task<bool> SyncAllEntitiesSince(DateTime specifiedDate)
    {
        var completed = false;
        try
        {
            await SyncAllCharactersSince(specifiedDate);
            await SyncAllHistoricalEventsSince(specifiedDate);
            await SyncAllPlanetsSince(specifiedDate);
            await SyncAllSpeciesSince(specifiedDate);
            completed = true;
        }
        catch (Exception ex)
        {
            LogMessage($"Error: {ex.Message}", LogLevel.Error);
        }
        return completed;
    }

    public async Task SyncAllCharacters()
    {
        LogMessage("Syncing all characters", LogLevel.Information);
        var characterDtos = await _characterClient.GetAll();
        if(characterDtos == null)
        {
            LogMessage("No characters found", LogLevel.Warning);
            return;
        }
        await ProcessCharacters(characterDtos);
    }

    public async Task SyncAllCharactersSince(DateTime specifiedDate)
    {
        LogMessage("Syncing all characters since", LogLevel.Information);
        var characterDtos = await _characterClient.GetAllUpdatedAfter(specifiedDate);
        if (characterDtos == null)
        {
            LogMessage("No characters found", LogLevel.Warning);
            return;
        }
        await ProcessCharacters(characterDtos);
    }

    private async Task ProcessCharacters(IEnumerable<CharacterDto> characterDtos)
    {
        LogMessage("Processing characters", LogLevel.Information);
        int successfulSyncs = 0;
        foreach (var characterDto in characterDtos)
        {
            if (await blobStorageService.UploadJsonObjectAsync(characterDto, "characters", $"{characterDto.Id}-{characterDto.GivenName}.json"))
            {
                successfulSyncs++;
            }
        }
        LogMessage($"Sync'ed {successfulSyncs} characters", LogLevel.Information);
    }

    public async Task SyncAllHistoricalEvents()
    {
        LogMessage("Syncing all historical events", LogLevel.Information);
        var historicalEventDtos = await _historicalEventClient.GetAll();
        if(historicalEventDtos == null)
        {
            LogMessage("No historical events found", LogLevel.Warning);
            return;
        }
        await ProcessHistoricalEvents(historicalEventDtos);
    }

    public async Task SyncAllHistoricalEventsSince(DateTime specifiedDate)
    {
        LogMessage("Syncing all historical events since", LogLevel.Information);
        var historicalEventDtos = await _historicalEventClient.GetAllUpdatedAfter(specifiedDate);
        if (historicalEventDtos == null)
        {
            LogMessage("No historical events found", LogLevel.Warning);
            return;
        }
        await ProcessHistoricalEvents(historicalEventDtos);
    }

    private async Task ProcessHistoricalEvents(IEnumerable<HistoricalEventDto> historicalEventDtos)
    {
        LogMessage("Processing historical events", LogLevel.Information);
        int successfulSyncs = 0;
        foreach (var historicalEventDto in historicalEventDtos)
        {
            if (await blobStorageService.UploadJsonObjectAsync(historicalEventDto, "historicalevents", $"{historicalEventDto.Id}-{historicalEventDto.Name}.json"))
            {
                successfulSyncs++;
            }
        }
        LogMessage($"Sync'ed {successfulSyncs} historical events", LogLevel.Information);
    }

    public async Task SyncAllPlanets()
    {
        LogMessage("Syncing all planets", LogLevel.Information);
        var planetDtos = await _planetClient.GetAll();
        if (planetDtos == null)
        {
            LogMessage("No planets found", LogLevel.Warning);
            return;
        }
        await ProcessPlanets(planetDtos);
    }

    public async Task SyncAllPlanetsSince(DateTime specifiedDate)
    {
        LogMessage("Syncing all planets since", LogLevel.Information);
        var planetDtos = await _planetClient.GetAllUpdatedAfter(specifiedDate);
        if (planetDtos == null)
        {
            LogMessage("No planets found", LogLevel.Warning);
            return;
        }
        await ProcessPlanets(planetDtos);
    }

    private async Task ProcessPlanets(IEnumerable<PlanetDto> planetDtos)
    {
        LogMessage("Processing planets", LogLevel.Information);
        int successfulSyncs = 0;
        foreach (var planetDto in planetDtos)
        {
            if (await blobStorageService.UploadJsonObjectAsync(planetDto, "planets", $"{planetDto.Id}-{planetDto.Name}.json"))
            {
                successfulSyncs++;
            }
        }
        LogMessage($"Sync'ed {successfulSyncs} planets", LogLevel.Information);
    }

    public async Task SyncAllSpecies()
    {
        LogMessage("Syncing all species", LogLevel.Information);
        var speciesDtos = await _speciesClient.GetAll();
        if (speciesDtos == null)
        {
            LogMessage("No species found", LogLevel.Warning);
            return;
        }
        await ProcessSpecies(speciesDtos);
    }

    public async Task SyncAllSpeciesSince(DateTime specifiedDate)
    {
        LogMessage("Syncing all species since", LogLevel.Information);
        var speciesDtos = await _speciesClient.GetAllUpdatedAfter(specifiedDate);
        if (speciesDtos == null)
        {
            LogMessage("No species found", LogLevel.Warning);
            return;
        }
        await ProcessSpecies(speciesDtos);
    }

    private async Task ProcessSpecies(IEnumerable<SpeciesDto> speciesDtos)
    {
        LogMessage("Processing species", LogLevel.Information);
        int successfulSyncs = 0;
        foreach (var speciesDto in speciesDtos)
        {
            if (await blobStorageService.UploadJsonObjectAsync(speciesDto, "species", $"{speciesDto.Id}-{speciesDto.Name}.json"))
            {
                successfulSyncs++;
            }
        }
        LogMessage($"Sync'ed {successfulSyncs} species", LogLevel.Information);
    }

    private void LogMessage(string message, LogLevel logLevel = LogLevel.Information)
    {
        var msg = $"Logger, UTC:{DateTime.UtcNow} => {message}";
        _logger.Log(logLevel, msg);
        System.Diagnostics.Debug.WriteLine(msg);
    }
}
