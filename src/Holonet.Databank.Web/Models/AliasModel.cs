using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Web.Models;

public class AliasModel
{
	public int Id { get; set; }
	
	[StringLength(150, ErrorMessage ="The Alias name must be no longer than {1} characters.")]
	public string Name { get; set; } = string.Empty;
	
}
