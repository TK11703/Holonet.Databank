using Blazored.Toast.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Web.Clients;
using Holonet.Databank.Web.Components.Shared;
using Holonet.Databank.Web.Models;
using Holonet.Databank.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Holonet.Databank.Web.Components.Pages.Characters;

public partial class ViewCharacter
{
	[Parameter]
	public int ID { get; set; }

	public CharacterModel Model { get; set; } = new();

	public DataRecordModel RecordModel { get; set; } = new();
	private EditContext EditContext { get; set; } = default!;

	public AppModal DeleteModal { get; set; } = default!;

	public AppModal AddRecordModal { get; set; } = default!;
	public int DeleteID { get; set; }

	[Inject]
	private CharacterClient CharacterClient { get; set; } = default!;

	[Inject]
	private NavigationManager NavigationManager { get; set; } = default!;
	[Inject]
	private UserService UserService { get; set; } = default!;

	[Inject]
	private IToastService ToastService { get; set; } = default!;

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		Model = await CharacterClient.Get(ID) ?? new();
		if (Model.Id.Equals(0))
		{
			NavigationManager.NavigateTo("/notfound", true);
		}
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		EditContext = new EditContext(RecordModel);
	}

	private MarkupString GetFormattedDescription(string input)
	{
		// Replace newline characters with <br> tags
		return new MarkupString(input.Replace("\n", "<br>"));
	}

	protected async Task HandleAddNew()
	{
		RecordModel.CharacterId = ID;
		if (UserService.IsUserAuthenticated())
		{
			RecordModel.UpdatedBy = new AuthorModel() { AzureId = UserService.GetAzureId() };
		}
		var completed = await CharacterClient.CreateDataRecord(ID, RecordModel);
		if(completed)
		{
			Model.DataRecords = await CharacterClient.GetDataRecords(ID) ?? Enumerable.Empty<DataRecordModel>();
			ToastService.ShowSuccess("New data record added successfully");
			AddRecordModal.Close();
			StateHasChanged();
		}
	}

	protected async Task HandleDelete()
	{
		var completed = await CharacterClient.DeleteRecord(ID, DeleteID);
		if (completed)
		{
			Model.DataRecords = Model.DataRecords.Where(x => !x.Id.Equals(DeleteID));
			ToastService.ShowSuccess("Deleted data record successfully");
			DeleteModal.Close();
			StateHasChanged();
		}
	}
}
