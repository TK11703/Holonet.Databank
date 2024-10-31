using Blazored.Toast.Services;
using Holonet.Databank.Web.Clients;
using Holonet.Databank.Web.Models;
using Holonet.Databank.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics.CodeAnalysis;

namespace Holonet.Databank.Web.Components.Pages.History;

public partial class CreateHistoricalEvent
{
	public HistoricalEventModel Model { get; set; } = new();

	private EditContext EditContext { get; set; } = default!;

	private ValidationMessageStore MessageStore { get; set; } = default!;

	IEnumerable<KeyValuePair<int, string>> Planets { get; set; } = [];

	IEnumerable<KeyValuePair<int, string>> Characters { get; set; } = [];

	[Inject]
	private CharacterClient CharacterClient { get; set; } = default!;

	[Inject]
	private HistoricalEventClient HistoricalEventClient { get; set; } = default!;

	[Inject]
	private UserService UserService { get; set; } = default!;

	[Inject]
	private PlanetClient PlanetClient { get; set; } = default!;

	[Inject]
	private IToastService ToastService { get; set; } = default!;

	[Inject]
	private NavigationManager NavigationManager { get; set; } = default!;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await LoadPlanets();
		await LoadCharacters();
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
			var result = await HistoricalEventClient.Create(Model);
			if (result > 0)
			{
				ToastService.ShowSuccess("Historical event created successfully");
				NavigationManager.NavigateTo("/historicalevents/index");
			}
			else
			{
				ToastService.ShowError("An error occurred and the historical event was not created.");
			}
		}
	}

	private async void HandleFieldChangedAsync([NotNull] object? sender, FieldChangedEventArgs e)
	{
		MessageStore.Clear(e.FieldIdentifier);
		if (e.FieldIdentifier.FieldName == nameof(Model.Name))
		{
			await DuplicateItemCheck(e.FieldIdentifier);
		}
		EditContext.NotifyValidationStateChanged();
	}

	private async Task DuplicateItemCheck(FieldIdentifier fieldIdentifier)
	{
		if (Model != null)
		{
			var exists = await HistoricalEventClient.Exists(Model.Id, Model.Name);
			if (exists)
			{
				MessageStore.Add(fieldIdentifier, "A historical event with this name already exists.");
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
			Planets = planets.Select(i => new KeyValuePair<int, string>(i.Id, i.Name));
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
			Characters = characters.Select(i => new KeyValuePair<int, string>(i.Id, $"{i.GivenName} {i.FamilyName}"));
		}
		else
		{
			Characters = [];
		}
	}
}
