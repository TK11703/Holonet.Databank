using Holonet.Databank.Web.Models;
using Microsoft.AspNetCore.Components;

namespace Holonet.Databank.Web.Components.Shared;

public partial class MultipleSpeciesSelector: ComponentBase
{
	[Parameter]
	public IEnumerable<SpeciesModel> Species { get; set; } = [];

	[Parameter]
	public List<int> SelectedSpeciesIds { get; set; } = [];

	private void UpdateSelection(int id, bool isSelected)
	{
		if (isSelected && !SelectedSpeciesIds.Contains(id))
		{
			SelectedSpeciesIds.Add(id);
		}
		else
		{
			SelectedSpeciesIds.Remove(id);
		}
	}
}
