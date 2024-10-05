using Blazored.Toast.Services;
using Holonet.Databank.Web.Clients;
using Holonet.Databank.Web.Models;
using Microsoft.AspNetCore.Components;

namespace Holonet.Databank.Web.Components.Pages.Planets;

public partial class UpdatePlanet
{
	[Parameter]
	public int ID { get; set; }

	public PlanetModel Model { get; set; } = new();

	[Inject]
	private PlanetClient PlanetClient { get; set; } = default!;

	[Inject]
	private IToastService ToastService { get; set; } = default!;

	[Inject]
	private NavigationManager NavigationManager { get; set; } = default!;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		var currentItem = await PlanetClient.Get(ID);
		if (currentItem != null)
		{
			Model = currentItem;
		}
	}

	private async Task Submit()
	{
		if (Model == null)
		{
			ToastService.ShowError("The form data was not found within the model on submit.");
		}
		else
		{
			var result = await PlanetClient.Update(Model, ID);
			if (result)
			{
				ToastService.ShowSuccess("Planet updated successfully");
				NavigationManager.NavigateTo($"/planets/{ID}");
			}
			else
			{
				ToastService.ShowError("An error occurred and the planet was not updated.");
			}
		}
	}
}
