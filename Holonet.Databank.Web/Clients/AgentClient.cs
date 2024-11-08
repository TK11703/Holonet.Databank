using Holonet.Databank.Web.Models;
using Holonet.Databank.Core.Dtos;
using Microsoft.Identity.Web;

namespace Holonet.Databank.Web.Clients;

public class AgentClient : ClientBase
{
	private readonly HttpClient _httpClient;
	private readonly ILogger<AgentClient> _logger;

	public AgentClient(HttpClient httpClient, ILogger<AgentClient> logger, ITokenAcquisition tokenAcquisition, IConfiguration configuration) : base(tokenAcquisition, configuration)
	{
		_httpClient = httpClient;
		_logger = logger;
		PerformClientChecks();
	}

	private void PerformClientChecks()
	{
		if (_httpClient.BaseAddress == null)
		{
			_logger.LogError("BaseAddress of AgentClient cannot be null.");
		}
	}

	public async Task<string> NewChat(Guid azureId)
	{
		if (base.RequiresBearToken())
		{
			await AcquireBearerTokenForClient(_httpClient);
		}

		using HttpResponseMessage response = await _httpClient.GetAsync($"NewChat/{azureId}");
		if (response.IsSuccessStatusCode)
		{
			var chatResponse = await response.Content.ReadFromJsonAsync<ChatResponseDto>();
			if (chatResponse != null)
			{
				return chatResponse.Response;
			}
		}
		_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		return string.Empty;
	}

	public async Task<string> ExecuteChat(ChatRequestModel request)
	{
		if (base.RequiresBearToken())
		{
			await AcquireBearerTokenForClient(_httpClient);
		}

		var chatRequestDto = request.ToChatRequestDto();
		using HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"ExecuteChat", chatRequestDto);
		if (response.IsSuccessStatusCode)
		{
			var chatResponse = await response.Content.ReadFromJsonAsync<ChatResponseDto>();
			if (chatResponse != null)
			{
				return chatResponse.Response;
			}
		}
		_logger.LogError("Http Status:{StatusCode}{Newline}Http Message: {Content}", response.StatusCode, Environment.NewLine, await response.Content.ReadAsStringAsync());
		return string.Empty;
	}
}
