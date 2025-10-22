using Azure.Storage.Queues;
using Holonet.Databank.AppFunctions.Clients;
using Holonet.Databank.AppFunctions.Configuration;
using Holonet.Databank.Core.Dtos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;

namespace Holonet.Databank.AppFunctions.Functions;

public class StaleShardQueueReviewer(ILoggerFactory loggerFactory, IOptions<AppSettings> appSettings, AIServiceClient serviceClient, CharacterClient characterClient, PlanetClient planetClient, SpeciesClient speciesClient, HistoricalEventClient historicalEventClient)
    : DataRecordDtoProcessor(loggerFactory, appSettings, serviceClient, characterClient, planetClient, speciesClient, historicalEventClient)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<StaleShardQueueReviewer>();
    private readonly AppSettings _settings = appSettings.Value;

    [Function("StaleShardQueueReviewer")]
    public async Task Run([TimerTrigger("0 0 8 * * *")] TimerInfo myTimer)
    {
        DateTime executedOn = DateTime.UtcNow;
        _logger.LogInformation("Holonet.Databank.Functions StaleShardQueueReviewer was triggered at: {ExecutionTime}.", executedOn);

        QueueClient queueClient = new QueueClient(_settings.StorageQueue.DefaultConnection, _settings.StorageQueue.QueueName);
        await queueClient.CreateIfNotExistsAsync();
        if (await queueClient.ExistsAsync())
        {
            int maxMessages = 10; // You can adjust this number (max 32 per call)
            var response = await queueClient.ReceiveMessagesAsync(maxMessages);

            if (response.Value.Length == 0)
            {
                _logger.LogInformation("Holonet.Databank.Functions StaleShardQueueReviewer - No messages found in the queue.");
                return;
            }

            foreach (var message in response.Value)
            {
                try
                {
                    DataRecordFunctionDto? record = JsonSerializer.Deserialize<DataRecordFunctionDto?>(message.Body.ToString());
                    if (record == null)
                    {
                        _logger.LogError("Holonet.Databank.Functions StaleShardQueueReviewer error: Invalid data record.");
                    }
                    else if (record.Id.Equals(0))
                    {
                        _logger.LogError("Holonet.Databank.Functions StaleShardQueueReviewer error: Invalid data record - no item ID.");
                    }
                    else if (!string.IsNullOrWhiteSpace(record.Data))
                    {
                        _logger.LogInformation("Holonet.Databank.Functions StaleShardQueueReviewer Skipped: User provided content (Retrieval not necessary).");
                    }
                    else if (string.IsNullOrWhiteSpace(record.Shard))
                    {
                        _logger.LogWarning("Holonet.Databank.Functions StaleShardQueueReviewer Warning: No shard provided for record ID: {RecordId}", record.Id);
                    }
                    else
                    {
                        await ProcessDataRecordDtoAsync(record);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Holonet.Databank.Functions StaleShardQueueReviewer - Error processing message.");
                    // Optionally: move to a poison queue or log for retry
                }
                finally
                {
                    await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                }
            }
        }
        else
        {
            _logger.LogWarning("Holonet.Databank.Functions StaleShardQueueReviewer error stating the Queue does not exist.");
        }        
    }
}