using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Web.Models;

public class PlanetModel
{
	public int Id { get; set; }

	[Required]
	[StringLength(150)]
	public string Name { get; set; } = string.Empty;

	[Url]
	[StringLength(500)]
	public string? Shard { get; set; }

	public List<AliasModel> Aliases { get; set; } = new();

	public List<int> DataRecordIds { get; set; } = [];
	public IEnumerable<DataRecordModel> DataRecords { get; set; } = [];

	public AuthorModel? UpdatedBy { get; set; }

	public DateTime? UpdatedOn { get; set; }
}
