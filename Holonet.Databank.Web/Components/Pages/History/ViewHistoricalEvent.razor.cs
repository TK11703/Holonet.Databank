using Blazored.Toast.Services;
using Holonet.Databank.Web.Clients;
using Holonet.Databank.Web.Components.Shared;
using Holonet.Databank.Web.Models;
using Holonet.Databank.Web.Services;
using Markdig.Extensions.AutoLinks;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Holonet.Databank.Web.Components.Pages.History;

public partial class ViewHistoricalEvent
{
	[Parameter]
	public int ID { get; set; }

	public HistoricalEventModel Model { get; set; } = new();

	public DataRecordModel RecordModel { get; set; } = new();
	private EditContext EditContext { get; set; } = default!;

	public AppModal DeleteModal { get; set; } = default!;
	public AppModal AddRecordModal { get; set; } = default!;
	public int DeleteID { get; set; }

	[Inject]
	private HistoricalEventClient HistoricalEventClient { get; set; } = default!;

	[Inject]
	private NavigationManager Navigation { get; set; } = default!;
	[Inject]
	private UserService UserService { get; set; } = default!;

	[Inject]
	private IToastService ToastService { get; set; } = default!;

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		var requestedItem = await HistoricalEventClient.Get(ID);
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

	private MarkupString GetFormattedDescription(string input)
	{
        var pipeline = new MarkdownPipelineBuilder().UseAutoLinks(new AutoLinkOptions { OpenInNewWindow = true }).Build();
        return new MarkupString(Markdown.ToHtml(markdown: input, pipeline: pipeline));
    }

	protected async Task HandleAddNew()
	{
		RecordModel.HistoricalEventId = ID;
		if (UserService.IsUserAuthenticated())
		{
			RecordModel.UpdatedBy = new AuthorModel() { AzureId = UserService.GetAzureId() };
		}
		var completed = await HistoricalEventClient.CreateDataRecord(ID, RecordModel);
		if (completed)
		{
			Model.DataRecords = await HistoricalEventClient.GetDataRecords(ID) ?? Enumerable.Empty<DataRecordModel>();
			ToastService.ShowSuccess("New data record added successfully");
            ResetModal();
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
		var completed = await HistoricalEventClient.DeleteRecord(ID, DeleteID);
		if (completed)
		{
			Model.DataRecords = Model.DataRecords.Where(x => !x.Id.Equals(DeleteID));
			ToastService.ShowSuccess("Deleted data record successfully");
			DeleteModal.Close();
			StateHasChanged();
		}
	}
}
