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
		await base.OnInitializedAsync();
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
            Start = 0,
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
        PageRequest.Start = 0;

        await GetData();
    }

    private string SortIndicator(string sortField)
    {
        if (sortField.Equals(PageRequest.SortBy))
        {
			return PageRequest.SortDirection.ToLower().Equals("asc") ? "bi-sort-down-alt" : "bi-sort-down";
		}
        return string.Empty;
    }

    public async Task PageIndexChanged(int newPageNumber)
    {
        if (newPageNumber < 1 || newPageNumber > ResultPage.TotalPages)
        {
            return;
        }
		PageRequest.Start = ((newPageNumber - 1) * PageRequest.PageSize);
		await GetData();
        StateHasChanged();
    }
	private async Task FilterResults()
	{
		PageRequest.Start = 0;
		await GetData();
	}

	private RenderFragment<Models.SpeciesModel> SpeciesIdentification = species => builder =>
	{
		builder.OpenElement(0, "dl");
		builder.AddAttribute(1, "class", "row");

		builder.OpenElement(2, "dt");
		builder.AddAttribute(3, "class", "col-sm-3");
		builder.AddContent(4, "Species:");
		builder.CloseElement();

		builder.OpenElement(5, "dd");
		builder.AddAttribute(6, "class", "col-sm-9");
		builder.AddContent(7, species.Name);
		builder.CloseElement();

		builder.CloseElement();
	};
}
