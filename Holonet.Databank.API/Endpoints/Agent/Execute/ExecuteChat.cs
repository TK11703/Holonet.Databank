using Holonet.Databank.API.Filters;
using Holonet.Databank.Application.AICapabilities;
using Holonet.Databank.Application.AICapabilities.Plugins;
using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;

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
			//var azureSearchExtensionConfiguration = new AzureSearchChatExtensionConfiguration
			//{
			//	SearchEndpoint = new Uri(Environment.GetEnvironmentVariable("AZURE_AI_SEARCH_ENDPOINT")),
			//	Authentication = new OnYourDataApiKeyAuthenticationOptions(Environment.GetEnvironmentVariable("AZURE_AI_SEARCH_API_KEY")),
			//	IndexName = Environment.GetEnvironmentVariable("AZURE_AI_SEARCH_INDEX")
			//};

			//var chatExtensionsOptions = new AzureChatExtensionsOptions { Extensions = { azureSearchExtensionConfiguration } };
			//var executionSettings = new OpenAIPromptExecutionSettings { AzureChatExtensionsOptions = chatExtensionsOptions };

			//var result = await kernel.InvokePromptAsync("What are my available health plans?", new(executionSettings));

			var chatHistory = chatHistoryManager.GetOrCreateChatHistory(chatRequest.AzureId.ToString());

			//kernel.ImportPluginFromObject(new DBQueryPlugin(_configuration));
			chatHistory.AddUserMessage(chatRequest.Prompt);

			ChatMessageContent? result = await chat.GetChatMessageContentAsync(chatHistory,
				  executionSettings: new OpenAIPromptExecutionSettings { Temperature = 0.8, TopP = 0.0, ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions },
				  kernel: kernel);

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