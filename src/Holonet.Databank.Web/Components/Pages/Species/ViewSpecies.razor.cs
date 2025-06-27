using Blazored.Toast.Services;
using Holonet.Databank.Web.Clients;
using Holonet.Databank.Web.Components.Shared;
using Holonet.Databank.Web.Models;
using Holonet.Databank.Web.Services;
using Markdig.Extensions.AutoLinks;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Holonet.Databank.Web.Components.Pages.Species;

public partial class ViewSpecies
{
	[Parameter]
	public int ID { get; set; }

	public SpeciesModel Model { get; set; } = new();

	public DataRecordModel RecordModel { get; set; } = new();
	private EditContext EditContext { get; set; } = default!;

	public AppModal DeleteModal { get; set; } = default!;
	public AppModal AddRecordModal { get; set; } = default!;
	public int DeleteID { get; set; }

	public int ProcessRecordID { get; set; }
	public string ProcessRecordShard { get; set; } = string.Empty;

    [Inject]
	private SpeciesClient SpeciesClient { get; set; } = default!;

    [Inject]
    private FunctionAppClient FunctionAppClient { get; set; } = default!;

    [Inject]
	private NavigationManager Navigation { get; set; } = default!;
	[Inject]
	private UserService UserService { get; set; } = default!;

	[Inject]
	private IToastService ToastService { get; set; } = default!;

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		var requestedItem = await SpeciesClient.Get(ID);
		if (requestedItem == null)
		{
			Navigation.NavigateTo("/notfound", true);
		}
		else
		{
			Model = requestedItem;
		}
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		EditContext = new EditContext(RecordModel);
	}

    private MarkupString GetFormattedDescription(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return new MarkupString(string.Empty);
        }
        var pipeline = new MarkdownPipelineBuilder().UseAutoLinks(new AutoLinkOptions { OpenInNewWindow = true }).Build();
        return new MarkupString(Markdown.ToHtml(markdown: input, pipeline: pipeline));
    }
	protected async Task HandleAddNew()
	{
		RecordModel.SpeciesId = ID;
		if (UserService.IsUserAuthenticated())
		{
			RecordModel.CreatedBy = new AuthorModel() { AzureId = UserService.GetAzureId() };
		}
		if (string.IsNullOrEmpty(RecordModel.Shard) && string.IsNullOrEmpty(RecordModel.Data))
		{
			ToastService.ShowError("Either Shard or Data must be provided.");
            ResetModal();
        }
		else if (!string.IsNullOrEmpty(RecordModel.Shard) && await RecordExists(RecordModel.Shard))
		{
			ToastService.ShowError("A data record with this Shard already exists. Please use a different Shard.");
            ResetModal();
        }
		else
		{
			int newId = await SpeciesClient.CreateDataRecord(ID, RecordModel);
			if (newId > 0)
			{
				Model.DataRecords = await SpeciesClient.GetDataRecords(ID) ?? Enumerable.Empty<DataRecordModel>();
				ToastService.ShowSuccess("New data record added successfully");
				ResetModal();
			}
			else
			{
				ToastService.ShowError("Failed to add new data record. Please try again.");
				ResetModal();
			}
		}
	}
	protected async Task RequestDataRecordProcessing()
	{
		var dataRecord = new DataRecordModel() { SpeciesId = ID, Id = ProcessRecordID, Shard = ProcessRecordShard };
		if (string.IsNullOrEmpty(dataRecord.Shard))
		{			
			ToastService.ShowError("Could not request further processing, because the Shard cannot be empty.");
			return;
		}
        var completed = await FunctionAppClient.ProcessNewDataRecord(dataRecord);
        if (completed)
		{
            ToastService.ShowSuccess("Request to process data record shard was sent");
        }
        else
        {
            ToastService.ShowError("Request for data processing failed.");
        }
    }

    protected void ResetModal()
    {
        RecordModel = new();
        AddRecordModal.Close();
        StateHasChanged();
    }

    protected async Task HandleDelete()
	{
		var completed = await SpeciesClient.DeleteRecord(ID, DeleteID);
		if (completed)
		{
			Model.DataRecords = Model.DataRecords.Where(x => !x.Id.Equals(DeleteID));
			ToastService.ShowSuccess("Deleted data record successfully");
			DeleteModal.Close();
			StateHasChanged();
		}
	}

    private async Task<bool> RecordExists(string shard)
    {
        return await SpeciesClient.DataRecordExists(ID, shard);
    }
}