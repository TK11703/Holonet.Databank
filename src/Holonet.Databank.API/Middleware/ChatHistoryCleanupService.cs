using Holonet.Databank.Application.AICapabilities;

namespace Holonet.Databank.API.Middleware;

public class ChatHistoryCleanupService(IChatHistoryManager chatHistoryManager) : BackgroundService
{
	private readonly IChatHistoryManager _chatHistoryManager = chatHistoryManager;
	private readonly TimeSpan _interval = TimeSpan.FromHours(1); // Adjust as needed

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			_chatHistoryManager.CleanupOldHistories();
			await Task.Delay(_interval, stoppingToken);
		}
	}
}