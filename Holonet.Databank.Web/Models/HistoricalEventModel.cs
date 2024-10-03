using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Web.Models;

public class HistoricalEventModel
{	public int Id { get; set; }

	[Required]
	[StringLength(150)]
	public string Name { get; set; } = string.Empty;

	public string? Description { get; set; }

	[Url]
	[StringLength(500)]
	public string? Shard { get; set; }

	[StringLength(200)]
	public string? DatePeriod { get; set; }

	public IEnumerable<int> CharacterIds { get; set; } = [];
	public IEnumerable<CharacterModel> Characters { get; set; } = [];
	public IEnumerable<int> PlanetIds { get; set; } = [];
	public IEnumerable<PlanetModel> Planets { get; set; } = [];

	public AuthorModel? UpdatedBy { get; set; }

	public DateTime? UpdatedOn { get; set; }
}
