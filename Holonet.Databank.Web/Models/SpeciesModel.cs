using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Web.Models;

public class SpeciesModel
{
	public int Id { get; set; }

	[Required]
	[StringLength(150)]
	public string Name { get; set; } = string.Empty;

	public string? Description { get; set; }

	[Url]
	[StringLength(500)]
	public string? Shard { get; set; }

	public List<AliasModel> Aliases { get; set; } = new();

	public AuthorModel? UpdatedBy { get; set; }

	public DateTime? UpdatedOn { get; set; }
}
