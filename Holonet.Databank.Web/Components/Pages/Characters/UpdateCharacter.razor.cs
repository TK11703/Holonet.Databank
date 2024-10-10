using Blazored.Toast.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Web.Clients;
using Holonet.Databank.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics.CodeAnalysis;

namespace Holonet.Databank.Web.Components.Pages.Characters;

public partial class UpdateCharacter
{
	[Parameter]
	public int ID { get; set; }

	public CharacterModel Model { get; set; } = new();

	private EditContext EditContext { get; set; } = default!;

	private ValidationMessageStore MessageStore { get; set; } = default!;

	public IEnumerable<PlanetModel> Planets { get; set; } = [];

	public IEnumerable<KeyValuePair<int, string>> Species { get; set; } = [];

	[Inject]
	private CharacterClient CharacterClient { get; set; } = default!;

	[Inject]
	private PlanetClient PlanetClient { get; set; } = default!;

	[Inject]
	private SpeciesClient SpeciesClient { get; set; } = default!;

	[Inject]
	private IToastService ToastService { get; set; } = default!;

	[Inject]
	private NavigationManager NavigationManager { get; set; } = default!;

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		Model = await CharacterClient.Get(ID) ?? new();
		if (Model.Id.Equals(0))
		{
			NavigationManager.NavigateTo("/notfound", true);
		}
		else
		{
			EditContext = new EditContext(Model);
			MessageStore = new ValidationMessageStore(EditContext);
			EditContext.OnFieldChanged += HandleFieldChangedAsync;
		}
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await LoadPlanets();
		await LoadSpecies();
	}

	private async Task Submit()
	{
		if (Model == null)
		{
			ToastService.ShowError("The form was missing the required data. Please ensure the fields contain data and try again.");
		}
		else
		{
			var result = await CharacterClient.Update(Model, ID);
			if (result)
			{
				ToastService.ShowSuccess("Character updated successfully");
				//NavigationManager.NavigateTo($"/characters/update/{ID}");
			}
			else
			{
				ToastService.ShowError("An error occurred and the character was not updated.");
			}
		}
	}

	private async void HandleFieldChangedAsync([NotNull] object? sender, FieldChangedEventArgs e)
	{
		MessageStore.Clear(e.FieldIdentifier);
		if (e.FieldIdentifier.FieldName == nameof(Model.FirstName) && !string.IsNullOrEmpty(Model.LastName) && Model.PlanetId > 0)
		{
			await DuplicateItemCheck(e.FieldIdentifier);
		}
		EditContext.NotifyValidationStateChanged();
	}

	private async Task DuplicateItemCheck(FieldIdentifier fieldIdentifier)
	{
		if (Model != null)
		{
			var exists = await CharacterClient.Exists(Model.Id, Model.FirstName, Model.LastName, Model.PlanetId);
			if (exists)
			{
				MessageStore.Add(fieldIdentifier, "A character with this name and (potential planet) already exists.");
			}
		}
	}

	private async Task RefreshPlanets()
	{
		await LoadPlanets();
	}

	private async Task RefreshSpecies()
	{
		await LoadSpecies();
	}

	private async Task LoadPlanets()
	{
		var planets = await PlanetClient.GetAll();
		if (planets != null)
		{
			Planets = planets;
		}
		else
		{
			Planets = [];
		}
	}

	private async Task LoadSpecies()
	{
		var species = await SpeciesClient.GetAll();
		if (species != null)
		{
			Species = species.Select(s => new KeyValuePair<int, string>(s.Id, s.Name));
		}
		else
		{
			Species = [];
		}
	}
}
