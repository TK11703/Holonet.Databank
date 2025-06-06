﻿using Blazored.Toast.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Web.Clients;
using Holonet.Databank.Web.Models;
using Holonet.Databank.Web.Services;
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

	public IEnumerable<KeyValuePair<int, string>> Species { get; set; } = [];

	[Inject]
    private CharacterClient CharacterClient { get; set; } = default!;

	[Inject]
	private UserService UserService { get; set; } = default!;

	[Inject]
	private PlanetClient PlanetClient { get; set; } = default!;

	[Inject]
	private SpeciesClient SpeciesClient { get; set; } = default!;

	[Inject]
    private IToastService ToastService { get; set; } = default!;

	[Inject]
    private NavigationManager Navigation { get; set; } = default!;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await LoadPlanets();
		await LoadSpecies();
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
			if (UserService.IsUserAuthenticated())
			{
				Model.UpdatedBy = new AuthorModel() { AzureId = UserService.GetAzureId() };
			}
			var result = await CharacterClient.Create(Model);
			if (result > 0)
			{
				ToastService.ShowSuccess("Character created successfully");
				Navigation.NavigateTo("/characters/index");
			}
            else
            {
				ToastService.ShowError("An error occurred and the character was not created.");
			}
		}
    }

	private async void HandleFieldChangedAsync(object? sender, FieldChangedEventArgs e)
	{
		MessageStore.Clear(e.FieldIdentifier);
		if (e.FieldIdentifier.FieldName == nameof(Model.GivenName) || e.FieldIdentifier.FieldName == nameof(Model.FamilyName) || e.FieldIdentifier.FieldName == nameof(Model.PlanetId))
		{
			if (!string.IsNullOrEmpty(Model.GivenName) && !string.IsNullOrEmpty(Model.FamilyName) && Model.PlanetId > 0)
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
			var exists = await CharacterClient.Exists(Model.Id, Model.GivenName, Model.FamilyName, Model.PlanetId);
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
			Species = species.Select(s => new KeyValuePair<int, string>(s.Id, s.Name));
		}
		else
		{
			Species = [];
		}
	}
}
