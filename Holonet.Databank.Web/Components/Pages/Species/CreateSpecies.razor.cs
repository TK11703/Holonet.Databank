using Blazored.Toast.Services;
using Holonet.Databank.Web.Clients;
using Holonet.Databank.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics.CodeAnalysis;

namespace Holonet.Databank.Web.Components.Pages.Species;

public partial class CreateSpecies
{
	public SpeciesModel Model { get; set; } = new();

	private EditContext EditContext { get; set; } = default!;

	private ValidationMessageStore MessageStore { get; set; } = default!;

	[Inject]
	private SpeciesClient SpeciesClient { get; set; } = default!;

	[Inject]
	private IToastService ToastService { get; set; } = default!;

	[Inject]
	private NavigationManager NavigationManager { get; set; } = default!;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		EditContext = new EditContext(Model);
		MessageStore = new ValidationMessageStore(EditContext);
		EditContext.OnFieldChanged += HandleFieldChangedAsync;
	}

	private async Task Submit()
	{
		if (Model == null)
		{
			ToastService.ShowError("The form was missing the required data. Please ensure the fields contain data and try again.");
		}
		else
		{
			var result = await SpeciesClient.Create(Model);
			if (result > 0)
			{
				ToastService.ShowSuccess("Species created successfully");
				NavigationManager.NavigateTo("/species/index");
			}
			else
			{
				ToastService.ShowError("An error occurred and the species was not created.");
			}
		}
	}

	private async void HandleFieldChangedAsync([NotNull] object? sender, FieldChangedEventArgs e)
	{
		MessageStore.Clear(e.FieldIdentifier);
		if (e.FieldIdentifier.FieldName == nameof(Model.Name) && !string.IsNullOrEmpty(Model.Name))
		{
			await DuplicateItemCheck(e.FieldIdentifier);
		}
		EditContext.NotifyValidationStateChanged();
	}

	private async Task DuplicateItemCheck(FieldIdentifier fieldIdentifier)
	{
		if (Model != null)
		{
			var exists = await SpeciesClient.Exists(Model.Id, Model.Name);
			if (exists)
			{
				MessageStore.Add(fieldIdentifier, "A species with this name already exists.");
			}
		}
	}
}
