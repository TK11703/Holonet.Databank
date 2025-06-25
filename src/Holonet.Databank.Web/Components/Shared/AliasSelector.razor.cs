using Holonet.Databank.Web.Models;
using Microsoft.AspNetCore.Components;

namespace Holonet.Databank.Web.Components.Shared;

public partial class AliasSelector : ComponentBase
{
	[Parameter]
	public List<AliasModel> Aliases { get; set; } = new();

	public AliasModel NewAlias { get; set; } = new();

	private void AddNewAlias()
	{
		if (!string.IsNullOrEmpty(NewAlias.Name) && !Aliases.Exists(x => x.Name.Equals(NewAlias.Name, StringComparison.CurrentCultureIgnoreCase)))
		{
			Aliases.Add(new AliasModel { Name = NewAlias.Name });
			NewAlias = new();
		}
	}

	private void RemoveAlias(string name)
	{
		if(Aliases != null && Aliases.Count > 0 && Aliases.Exists(x=>x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)))
		{
			Aliases.RemoveAll(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
		}
	}
}
