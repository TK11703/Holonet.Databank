using Holonet.Databank.Application.AICapabilities;
using Holonet.Databank.Core.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Holonet.Databank.API.Endpoints.Agent.New;

public class NewChat : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet($"/Agent/NewChat/{{id}}", HandleAsync)
			.WithTags(Tags.Agent);
	}
	protected virtual async Task<Results<Ok<ChatResponseDto>, ProblemHttpResult>> HandleAsync(Guid id, [FromServices] Kernel kernel, [FromServices] IChatCompletionService chat, [FromServices] IChatHistoryManager chatHistoryManager)
	{
		try
		{
			var completed = chatHistoryManager.ClearChatHistory(id.ToString());
			if(!completed)
			{
				return TypedResults.Problem("Failed to clear chat history");
			}

            var chatHistory = chatHistoryManager.GetOrCreateChatHistory(id.ToString());
			ChatMessageContent? result = await chat.GetChatMessageContentAsync(chatHistory, kernel: kernel);

			if(result == null || string.IsNullOrEmpty(result.Content))
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