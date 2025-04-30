
namespace Holonet.Databank.Web.Models;

public class PlanetViewingModel
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;

    public string? LatestShard { get; set; } = string.Empty;

    public AuthorModel? UpdatedBy { get; set; }

	public DateTime? UpdatedOn { get; set; }
}
