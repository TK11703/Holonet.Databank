using Blazored.Toast.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Web.Clients;
using Holonet.Databank.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics.CodeAnalysis;

namespace Holonet.Databank.Web.Components.Pages.Characters;

public partial class CreateCharacter
{
    public CharacterModel Model { get; set; } = new();

	private EditContext EditContext { get; set; } = default!;

	private ValidationMessageStore MessageStore { get; set; } = default!;

	public IEnumerable<PlanetModel> Planets { get; set; } = [];

	public IEnumerable<SpeciesModel> Species { get; set; } = [];

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

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await LoadPlanets();
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
			var result = await CharacterClient.Create(Model);
			if (result > 0)
			{
				ToastService.ShowSuccess("Character created successfully");
				NavigationManager.NavigateTo("/characters/index");
			}
            else
            {
				ToastService.ShowError("An error occurred and the character was not created.");
			}
		}
    }

	private async void HandleFieldChangedAsync([NotNull] object? sender, FieldChangedEventArgs e)
	{
		MessageStore.Clear(e.FieldIdentifier);
		if (e.FieldIdentifier.FieldName == nameof(Model.FirstName) || e.FieldIdentifier.FieldName == nameof(Model.LastName) || e.FieldIdentifier.FieldName == nameof(Model.PlanetId))
		{
			if (!string.IsNullOrEmpty(Model.FirstName) && !string.IsNullOrEmpty(Model.LastName) && Model.PlanetId > 0)
			{
				await DuplicateItemCheck(e.FieldIdentifier);
			}
		}
		EditContext.NotifyValidationStateChanged();
	}

	private async Task DuplicateItemCheck(FieldIdentifier fieldIdentifier)
	{
		if (Model == null)
		{
			ToastService.ShowError("The form data was not found to execute the check.");
		}
		else
		{
			var exists = await CharacterClient.Exists(Model.Id, Model.FirstName, Model.LastName, Model.PlanetId);
			if (exists)
			{
				MessageStore.Add(fieldIdentifier, "A character with this name and (potential home planet) already exist.");
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
        if(planets != null)
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
			Species = species;
		}
		else
		{
			Species = [];
		}
	}
}
