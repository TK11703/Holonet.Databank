using Microsoft.AspNetCore.Components;

namespace Holonet.Databank.Web.Components.Shared;

public partial class CheckboxSelector : ComponentBase
{
	public string SearchQuery { get; set; } = string.Empty;

	[Parameter]
	public IEnumerable<KeyValuePair<int, string>> Options { get; set; } = [];

	[Parameter]
	public List<int> SelectedOptionIds { get; set; } = [];

	private void UpdateSelection(int id, bool isSelected)
	{
		if (isSelected && !SelectedOptionIds.Contains(id))
		{
			SelectedOptionIds.Add(id);
		}
		else
		{
			SelectedOptionIds.Remove(id);
		}
	}
}
