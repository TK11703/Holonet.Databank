using Blazored.Toast.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Models;
using Holonet.Databank.Web.Clients;
using Holonet.Databank.Web.Components.Shared;
using Holonet.Databank.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Reflection;

namespace Holonet.Databank.Web.Components.Pages.Characters;

public partial class IndexCharacters
{
    private PageResult<CharacterViewingModel> ResultPage { get; set; } = default!;

	private PageRequest PageRequest { get; set; } = default!;

    private EditContext EditContext { get; set; } = default!;
    public AppModal Modal { get; set; } = default!;
	public int DeleteID { get; set; }

	[Inject]
    private CharacterClient CharacterClient { get; set; } = default!;

	[Inject]
	private IToastService ToastService { get; set; } = default!;

	protected override async Task OnInitializedAsync()
    {
		await base.OnInitializedAsync();
        PageRequest = GetInitialPageObject();
        EditContext = new EditContext(PageRequest);
        await GetData();
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
            SortBy = nameof(CharacterDto.FamilyName),
            SortDirection = "asc"
        };
    }

    private async Task GetData()
    {
        ResultPage = await CharacterClient.GetAll(PageRequest);
        StateHasChanged();
    }

	protected async Task HandleDelete()
	{
		var completed = await CharacterClient.Delete(DeleteID);
		if (completed)
		{
			ToastService.ShowSuccess("Deleted character successfully");
			await GetData();
			Modal.Close();
		}
	}

    public async Task ClearFilter()
    {
        PageRequest.Filter = string.Empty;
        await GetData();
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
        if (sortField.Equals(PageRequest.SortBy) && !string.IsNullOrEmpty(PageRequest.SortDirection))
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

	private RenderFragment<Models.CharacterViewingModel> CharacterIdentification = character => builder =>
	{
		builder.OpenElement(0, "dl");
		builder.AddAttribute(1, "class", "row");

		builder.OpenElement(2, "dt");
		builder.AddAttribute(3, "class", "col-sm-3");
		builder.AddContent(4, "Character:");
		builder.CloseElement();

		builder.OpenElement(5, "dd");
		builder.AddAttribute(6, "class", "col-sm-9");
		builder.AddContent(7, character.GivenName + " " + character.FamilyName);
		builder.CloseElement();

		builder.CloseElement();
	};
}
