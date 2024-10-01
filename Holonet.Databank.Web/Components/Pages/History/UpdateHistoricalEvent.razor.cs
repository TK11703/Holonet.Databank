﻿using Blazored.Toast.Services;
using Holonet.Databank.Web.Clients;
using Holonet.Databank.Web.Models;
using Microsoft.AspNetCore.Components;

namespace Holonet.Databank.Web.Components.Pages.History;

public partial class UpdateHistoricalEvent
{
    [Parameter]
    public int ID { get; set; }

    public HistoricalEventModel Model { get; set; } = new();

	public IEnumerable<PlanetModel> Planets { get; set; } = [];

	public IEnumerable<CharacterModel> Characters { get; set; } = [];

	[Inject]
	private CharacterClient CharacterClient { get; set; } = default!;

	[Inject]
    private HistoricalEventClient HistoricalEventClient { get; set; } = default!;

    [Inject]
    private PlanetClient PlanetClient { get; set; } = default!;

    [Inject]
    private IToastService ToastService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        //await base.OnInitializedAsync();
		await LoadPlanets();
		await LoadCharacters();
		var currentItem = await HistoricalEventClient.Get(ID);
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
            var result = await HistoricalEventClient.Update(Model, ID);
            if (result)
            {
                ToastService.ShowSuccess("Historical event updated successfully");
                NavigationManager.NavigateTo("/planets");
            }
            else
            {
                ToastService.ShowError("An error occurred and the Historical event was not updated.");
            }
        }
    }

	private async Task RefreshPlanets()
	{
		await LoadPlanets();
	}

	private async Task RefreshCharacters()
	{
		await LoadCharacters();
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

	private async Task LoadCharacters()
	{
		var characters = await CharacterClient.GetAll();
		if (characters != null)
		{
			Characters = characters;
		}
		else
		{
			Characters = [];
		}
	}
}
