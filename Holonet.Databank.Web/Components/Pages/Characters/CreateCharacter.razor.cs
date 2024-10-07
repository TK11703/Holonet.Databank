﻿using Blazored.Toast.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Web.Clients;
using Holonet.Databank.Web.Models;
using Microsoft.AspNetCore.Components;

namespace Holonet.Databank.Web.Components.Pages.Characters;

public partial class CreateCharacter
{
    public CharacterModel Model { get; set; } = new();

    public IEnumerable<PlanetModel> Planets { get; set; } = [];

	public IEnumerable<SpeciesModel> Species { get; set; } = [];

	public bool CanSubmit { get; set; } = false;

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
        await LoadPlanets();
		//await base.OnInitializedAsync();
	}


    private async Task Submit()
    {
        if (Model == null)
        {
			ToastService.ShowError("The form data was not found within the model on submit.");
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

	private async Task Verify()
	{
		CanSubmit = false;
		if (Model == null)
		{
			ToastService.ShowError("The form data was not found to execute the check.");
		}
		else
		{
			var exists = await CharacterClient.Exists(Model.Id, Model.FirstName, Model.LastName, Model.PlanetId);
			if (exists)
			{
				ToastService.ShowWarning("A character with this name and (potential home planet) already exist.");
			}
			else
			{
				ToastService.ShowInfo("A record for this character has not yet been entered, so this request has been validated.");
				CanSubmit = true;
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
