using Holonet.Databank.API.Filters;
using Holonet.Databank.Application.AICapabilities;
using Holonet.Databank.Core.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Holonet.Databank.API.Endpoints.Agent.Execute;

public class ExecuteChat : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost($"/Agent/ExecuteChat", HandleAsync)
			.AddEndpointFilter<ValidatorFilter<ChatRequestDto>>()
			.WithTags(Tags.Agent);
	}
	protected virtual async Task<Results<Ok<ChatResponseDto>, ProblemHttpResult>> HandleAsync(ChatRequestDto chatRequest, Kernel kernel, IChatCompletionService chat, IChatHistoryManager chatHistoryManager)
	{
		try
		{
			ArgumentNullException.ThrowIfNull(chatRequest);

            var settings = new OpenAIPromptExecutionSettings { Temperature = 0.8, TopP = 0.0, ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };

            var chatHistory = chatHistoryManager.GetOrCreateChatHistory(chatRequest.AzureId.ToString());            
			chatHistory.AddUserMessage(chatRequest.Prompt);

			ChatMessageContent? result = await chat.GetChatMessageContentAsync(chatHistory, executionSettings: settings, kernel: kernel);

            if (result == null || string.IsNullOrEmpty(result.Content))
            {
                return TypedResults.Problem("Failed to get chat message content");
            }

            return TypedResults.Ok(new ChatResponseDto(result.Content));
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}