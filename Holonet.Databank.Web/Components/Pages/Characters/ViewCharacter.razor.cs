using Holonet.Databank.Web.Clients;
using Holonet.Databank.Web.Models;
using Microsoft.AspNetCore.Components;

namespace Holonet.Databank.Web.Components.Pages.Characters;

public partial class ViewCharacter
{
	[Parameter]
	public int ID { get; set; }

	public CharacterModel Model { get; set; } = new();

	[Inject]
	private CharacterClient CharacterClient { get; set; } = default!;

	[Inject]
	private NavigationManager NavigationManager { get; set; } = default!;

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		var requestedItem = await CharacterClient.Get(ID);
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
