using Blazored.Toast.Services;
using Holonet.Databank.Web.Clients;
using Holonet.Databank.Web.Models;
using Microsoft.AspNetCore.Components;

namespace Holonet.Databank.Web.Components.Pages.Species;

public partial class UpdateSpecies
{
	[Parameter]
	public int ID { get; set; }

	public bool CanSubmit { get; set; } = false;

	public SpeciesModel Model { get; set; } = new();

	[Inject]
	private SpeciesClient SpeciesClient { get; set; } = default!;

	[Inject]
	private IToastService ToastService { get; set; } = default!;

	[Inject]
	private NavigationManager NavigationManager { get; set; } = default!;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		var currentItem = await SpeciesClient.Get(ID);
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
			var result = await SpeciesClient.Update(Model, ID);
			if (result)
			{
				ToastService.ShowSuccess("Species updated successfully");
				NavigationManager.NavigateTo($"/species/{ID}");
			}
			else
			{
				ToastService.ShowError("An error occurred and the species was not updated.");
			}
		}
	}

	private async Task Verify()
	{
		CanSubmit = false;
		if (Model == null)
		{
			ToastService.ShowError("The form data was not found to execute the check.");
		}
		else
		{
			var exists = await SpeciesClient.Exists(Model.Id, Model.Name);
			if (exists)
			{
				ToastService.ShowWarning("A species with this name already exists.");
			}
			else
			{
				ToastService.ShowInfo("The data has been validated.");
				CanSubmit = true;
			}
		}
	}
}
