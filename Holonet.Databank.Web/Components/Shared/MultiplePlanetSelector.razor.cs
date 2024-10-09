using Holonet.Databank.Web.Clients;
using Holonet.Databank.Web.Models;
using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Holonet.Databank.Web.Components.Shared;

public partial class MultiplePlanetSelector: ComponentBase
{
    public IEnumerable<PlanetModel> Planets { get; set; } = [];

	[Parameter]
	public List<int> SelectedPlanetIds { get; set; } = [];

	private void UpdateSelection(int id, bool isSelected)
	{
		if (isSelected && !SelectedPlanetIds.Contains(id))
		{
			SelectedPlanetIds.Add(id);
		}
		else
		{
			SelectedPlanetIds.Remove(id);
		}
	}
}
