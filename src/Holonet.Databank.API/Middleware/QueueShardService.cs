using Azure.Identity;
using Azure.Storage.Queues;
using Holonet.Databank.API.Configuration;
using Holonet.Databank.Core.Dtos;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Holonet.Databank.API.Middleware
{
    public class QueueShardService(ILoggerFactory loggerFactory, IOptions<AppSettings> appSettings) : IQueueShardService
    {
        private readonly ILogger<QueueShardService> _logger = loggerFactory.CreateLogger<QueueShardService>();
        private readonly AppSettings _appSettings = appSettings.Value;

        public async Task QueueShardItem(DataRecordFunctionDto record)
        {
            DateTime executedOn = DateTime.UtcNow;

            _logger.LogInformation("Holonet.Databank.API QueueShardService is requested to queue a new data record for processing at: {ExecutionTime}.", executedOn);
            try
            {
                QueueClient queueClient = CreateQueueClient(_appSettings.StorageQueue.BaseUrl, _appSettings.StorageQueue.QueueName);

                // Ensure the queue exists
                await queueClient.CreateIfNotExistsAsync();

                if (await queueClient.ExistsAsync())
                {
                    string jsonMessage = JsonSerializer.Serialize(record);
                    await queueClient.SendMessageAsync(jsonMessage);
                    _logger.LogInformation("Holonet.Databank.API QueueShardService successfully queued a new data record.");
                }
                else
                {
                    _logger.LogError("Holonet.Databank.API QueueShardService error: Unable to queue the requested data record because the queue doesn't exist.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Holonet.Databank.API QueueShardService error queuing data record: {ErrorMessage}", ex.Message);
            }
        }

        public QueueClient CreateQueueClient(string baseUrl, string queueName)
        {
            if (_appSettings.StorageQueue.UseDevStorage)
            {
                // Local development using connection string
                return new QueueClient("UseDevelopmentStorage=true", "myqueue");
            }
            else if(_appSettings.StorageQueue.UseSAS)
            {
                var queueUri = new Uri(_appSettings.StorageQueue.SASUrl);
                return new QueueClient(queueUri);
            }
            else if (_appSettings.StorageQueue.UseConnectionString)
            {
                return new QueueClient(_appSettings.StorageQueue.DefaultConnection, _appSettings.StorageQueue.QueueName);
            }
            else
            {
                // Azure deployment using Managed Identity
                var queueUri = new Uri($"{baseUrl}/{queueName}");
                var credential = new DefaultAzureCredential();
                return new QueueClient(queueUri, credential);
            }
        }
    }
}
