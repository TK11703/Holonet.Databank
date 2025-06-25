using Microsoft.SemanticKernel.ChatCompletion;

namespace Holonet.Databank.Application.AICapabilities;
public interface IChatHistoryManager
{
	void CleanupOldHistories();
	ChatHistory GetOrCreateChatHistory(string sessionId);

	bool ClearChatHistory(string sessionId);
}