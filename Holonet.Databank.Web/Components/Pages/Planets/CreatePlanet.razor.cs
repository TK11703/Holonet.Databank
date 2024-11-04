using Blazored.Toast.Services;
using Holonet.Databank.Web.Clients;
using Holonet.Databank.Web.Models;
using Holonet.Databank.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics.CodeAnalysis;

namespace Holonet.Databank.Web.Components.Pages.Planets;

public partial class CreatePlanet
{
	public PlanetModel Model { get; set; } = new();

	private EditContext EditContext { get; set; } = default!;

	private ValidationMessageStore MessageStore { get; set; } = default!;

	[Inject]
	private PlanetClient PlanetClient { get; set; } = default!;

	[Inject]
	private UserService UserService { get; set; } = default!;

	[Inject]
	private IToastService ToastService { get; set; } = default!;

	[Inject]
	private NavigationManager Navigation { get; set; } = default!;

	protected override void OnInitialized()
	{
		base.OnInitialized();
		EditContext = new EditContext(Model);
		MessageStore = new ValidationMessageStore(EditContext);
		EditContext.OnFieldChanged += HandleFieldChangedAsync!;
	}

	private async Task Submit()
	{
		if (Model == null)
		{
			ToastService.ShowError("The form was missing the required data. Please ensure the fields contain data and try again.");
		}
		else
		{
			if (UserService.IsUserAuthenticated())
			{
				Model.UpdatedBy = new AuthorModel() { AzureId = UserService.GetAzureId() };
			}
			var result = await PlanetClient.Create(Model);
			if (result > 0)
			{
				ToastService.ShowSuccess("Planet created successfully");
				Navigation.NavigateTo("/planets/index");
			}
			else
			{
				ToastService.ShowError("An error occurred and the planet was not created.");
			}
		}
	}

	private async void HandleFieldChangedAsync(object? sender, FieldChangedEventArgs e)
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
			var exists = await PlanetClient.Exists(Model.Id, Model.Name);
			if (exists)
			{
				MessageStore.Add(fieldIdentifier, "A planet with this name already exists.");
			}
		}
	}
}
