using Blazored.Toast.Services;
using Holonet.Databank.Web.Clients;
using Holonet.Databank.Web.Models;
using Holonet.Databank.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.WebUtilities;
using System.Diagnostics.CodeAnalysis;

namespace Holonet.Databank.Web.Components.Pages.Species;

public partial class UpdateSpecies
{
	[Parameter]
	public int ID { get; set; }

	private string ReferrerPage { get; set; } = "species/index";

	public SpeciesModel Model { get; set; } = new();

	private EditContext EditContext { get; set; } = default!;

	private ValidationMessageStore MessageStore { get; set; } = default!;

	[Inject]
	private SpeciesClient SpeciesClient { get; set; } = default!;

	[Inject]
	private UserService UserService { get; set; } = default!;

	[Inject]
	private IToastService ToastService { get; set; } = default!;

	[Inject]
	private NavigationManager Navigation { get; set; } = default!;

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		Model = await SpeciesClient.Get(ID) ?? new();
		if (Model.Id.Equals(0))
		{
			Navigation.NavigateTo("/notfound", true);
		}
		else
		{
			EditContext = new EditContext(Model);
			MessageStore = new ValidationMessageStore(EditContext);
			EditContext.OnFieldChanged += HandleFieldChangedAsync!;
		}
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		var uri = new Uri(Navigation.Uri);
		var query = QueryHelpers.ParseQuery(uri.Query);
		if (query.TryGetValue("referrer", out var referrer))
		{
			ReferrerPage = referrer.FirstOrDefault() ?? "species/index";
		}
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
			var result = await SpeciesClient.Update(Model, ID);
			if (result)
			{
				ToastService.ShowSuccess("Species updated successfully");
			}
			else
			{
				ToastService.ShowError("An error occurred and the species was not updated.");
			}
		}
	}

	private void Cancel()
	{
		Navigation.NavigateTo(ReferrerPage);
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
			var exists = await SpeciesClient.Exists(Model.Id, Model.Name);
			if (exists)
			{
				MessageStore.Add(fieldIdentifier, "A species with this name already exists.");
			}
		}
	}
}
