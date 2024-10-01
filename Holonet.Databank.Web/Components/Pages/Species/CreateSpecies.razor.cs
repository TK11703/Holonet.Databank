using Blazored.Toast.Services;
using Holonet.Databank.Web.Clients;
using Holonet.Databank.Web.Models;
using Microsoft.AspNetCore.Components;

namespace Holonet.Databank.Web.Components.Pages.Species;

public partial class CreateSpecies
{
	public SpeciesModel Model { get; set; } = new();

	[Inject]
	private SpeciesClient SpeciesClient { get; set; } = default!;

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
			var result = await SpeciesClient.Create(Model);
			if (result > 0)
			{
				ToastService.ShowSuccess("Species created successfully");
				NavigationManager.NavigateTo("/species");
			}
			else
			{
				ToastService.ShowError("An error occurred and the species was not created.");
			}
		}
	}
}
