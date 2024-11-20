using Blazored.Toast.Services;
using Holonet.Databank.Web.Clients;
using Holonet.Databank.Web.Models;
using Holonet.Databank.Web.Services;
using Markdig;
using Markdig.Extensions.AutoLinks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Holonet.Databank.Web.Components.Pages.HolonetAgent;

public partial class Index
{
	public bool IsUserAuthenticated => UserService.IsUserAuthenticated();

	private ElementReference lastItemRef { get; set; } = default!;
	private bool shouldScroll { get; set; } = false;

	private bool isLoading { get; set; } = false;

	public ChatRequestModel Model { get; set; } = new();

	protected List<ChatResponseModel> ChatResponses { get; set; } = new();

	[Inject]
	private AgentClient AgentClient { get; set; } = default!;

	[Inject]
	private UserService UserService { get; set; } = default!;

	[Inject]
	private IToastService ToastService { get; set; } = default!;

	[Inject]
	private IJSRuntime JSRuntime { get; set; } = default!;

	protected override async Task OnAfterRenderAsync(bool firstRender) 
	{ 
		if (shouldScroll) 
		{ 
			shouldScroll = false; 
			await JSRuntime.InvokeVoidAsync("blazorExtensions.scrollToBottom", lastItemRef); 
		} 
	}

	protected async override Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		if(!IsUserAuthenticated)
		{
			ToastService.ShowError("You must be authenticated to use this feature.");
		}
		else
		{
			await StartNewChat();
		}
	}


	private async Task StartNewChat()
	{
		var response = await AgentClient.NewChat(UserService.GetAzureId());
		if (!string.IsNullOrEmpty(response))
		{
			ChatResponses = new List<ChatResponseModel>();
			ChatResponses.Add(new ChatResponseModel() { Result = response, Type = ChatResponseType.Agent });
			shouldScroll = true;
			StateHasChanged();
		}
		else
		{
			ToastService.ShowError("The request was not completed by the agent.");
		}
	}

	private async Task SendUserPrompt()
	{
		if (Model == null)
		{
			ToastService.ShowError("The chat request was missing the required data.");
		}
		else
		{
			isLoading = true;
			if (UserService.IsUserAuthenticated())
			{
				Model.AzureId = UserService.GetAzureId();
			}
			ChatResponses.Add(new ChatResponseModel() { Result = Model.Prompt, Type = ChatResponseType.User });
			shouldScroll = true;
			StateHasChanged();
			var result = await AgentClient.ExecuteChat(Model);
			Model.Prompt = string.Empty;
			if (!string.IsNullOrEmpty(result))
			{
                var pipeline = new MarkdownPipelineBuilder().UseAutoLinks(new AutoLinkOptions { OpenInNewWindow = true }).Build();
                ChatResponses.Add(new ChatResponseModel() { Result = Markdown.ToHtml(markdown: result, pipeline: pipeline), Type = ChatResponseType.Agent });
				shouldScroll = true;
			}
			else
			{	
				ToastService.ShowInfo("A response was not returned from the agent.");
			}
			isLoading = false;
			StateHasChanged();
		}
	}
}

