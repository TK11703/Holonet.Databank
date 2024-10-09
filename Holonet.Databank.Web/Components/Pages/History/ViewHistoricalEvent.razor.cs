using Holonet.Databank.Web.Clients;
using Holonet.Databank.Web.Models;
using Microsoft.AspNetCore.Components;

namespace Holonet.Databank.Web.Components.Pages.History;

public partial class ViewHistoricalEvent
{
	[Parameter]
	public int ID { get; set; }

	public HistoricalEventModel Model { get; set; } = new();

	[Inject]
	private HistoricalEventClient HistoricalEventClient { get; set; } = default!;

	[Inject]
	private NavigationManager NavigationManager { get; set; } = default!;

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		var requestedItem = await HistoricalEventClient.Get(ID);
		if (requestedItem == null)
		{
			NavigationManager.NavigateTo("/notfound", true);
		}
		else
		{
			Model = requestedItem;
		}
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();		
	}

	private MarkupString GetFormattedDescription()
	{
		// Replace newline characters with <br> tags
		var formattedText = Model.Description?.Replace("\n", "<br>");
		return new MarkupString(formattedText ?? string.Empty);
	}
}
