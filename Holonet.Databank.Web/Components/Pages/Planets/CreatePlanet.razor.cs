using Blazored.Toast.Services;
using Holonet.Databank.Web.Clients;
using Holonet.Databank.Web.Models;
using Microsoft.AspNetCore.Components;

namespace Holonet.Databank.Web.Components.Pages.Planets;

public partial class CreatePlanet
{
	public PlanetModel Model { get; set; } = new();

	[Inject]
	private PlanetClient PlanetClient { get; set; } = default!;

	[Inject]
	private IToastService ToastService { get; set; } = default!;

	[Inject]
	private NavigationManager NavigationManager { get; set; } = default!;

	private async Task Submit()
	{
		if (Model == null)
		{
			ToastService.ShowError("The form data was not found within the model on submit.");
		}
		else
		{
			var result = await PlanetClient.Create(Model);
			if (result > 0)
			{
				ToastService.ShowSuccess("Planet created successfully");
				NavigationManager.NavigateTo("/planets");
			}
			else
			{
				ToastService.ShowError("An error occurred and the planet was not created.");
			}
		}
	}
}
