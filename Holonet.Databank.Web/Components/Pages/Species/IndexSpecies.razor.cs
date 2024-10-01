using Blazored.Toast.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Models;
using Holonet.Databank.Web.Clients;
using Holonet.Databank.Web.Components.Shared;
using Holonet.Databank.Web.Models;
using Microsoft.AspNetCore.Components;

namespace Holonet.Databank.Web.Components.Pages.Species;

public partial class IndexSpecies
{
    private PageResult<SpeciesModel> ResultPage { get; set; } = default!;

    private PageRequest PageRequest { get; set; } = default!;
	public AppModal Modal { get; set; } = default!;
	public int DeleteID { get; set; }

	[Inject]
    private SpeciesClient SpeciesClient { get; set; } = default!;

	[Inject]
	private IToastService ToastService { get; set; } = default!;

	protected override async Task OnInitializedAsync()
    {
        PageRequest = GetInitialPageObject();
        await GetData();
    }

    private async Task GetData()
    {
        ResultPage = await SpeciesClient.GetAll(PageRequest);
    }

    private static PageRequest GetInitialPageObject()
    {
        return new PageRequest()
        {
            Start = 1,
            PageSize = 10,
            BeginDate = null,
            EndDate = null,
            Filter = null,
            SortBy = nameof(SpeciesDto.Name),
            SortDirection = "asc"
        };
    }

	protected async Task HandleDelete()
	{
		var completed = await SpeciesClient.Delete(DeleteID);
		if (completed)
		{
			ToastService.ShowSuccess("Deleted species successfully");
			await GetData();
			Modal.Close();
		}
	}

	private async Task Sort(string sortField)
    {
        if (PageRequest.SortBy == sortField)
        {
            PageRequest.SortDirection = PageRequest.SortDirection == "asc" ? "desc" : "asc";
        }
        else
        {
            PageRequest.SortBy = sortField;
            PageRequest.SortDirection = "asc";
        }
        PageRequest.Start = 1;

        await GetData();
    }

    private string SortIndicator(string sortField)
    {
        if (sortField.Equals(PageRequest.SortBy))
        {
            return PageRequest.SortDirection.Equals("Asc") ? "fa fa-sort-asc" : "fa fa-sort-desc";
        }
        return string.Empty;
    }

    public async Task PageIndexChanged(int newPageNumber)
    {
        if (newPageNumber < 1 || newPageNumber > ResultPage.TotalPages)
        {
            return;
        }
        PageRequest.Start = newPageNumber;
        await GetData();
        StateHasChanged();
    }
	private async Task FilterResults()
	{
		PageRequest.Start = 1;
		await GetData();
	}
}
