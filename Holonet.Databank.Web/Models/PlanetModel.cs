using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Web.Models;

public class PlanetModel
{
	[Required]
	public int Id { get; set; }

	[Required]
	[StringLength(150)]
	public string Name { get; set; } = string.Empty;

	public string? Description { get; set; }

	[Url]
	[StringLength(500)]
	public string? Shard { get; set; }

	[StringLength(250)]
	public string CreatedBy { get; set; } = string.Empty;

	[StringLength(250)]
	public string UpdatedBy { get; set; } = string.Empty;

	public DateTime CreatedOn { get; set; }
	public DateTime UpdatedOn { get; set; }
}
