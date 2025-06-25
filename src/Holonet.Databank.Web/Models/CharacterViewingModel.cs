
namespace Holonet.Databank.Web.Models;

public class CharacterViewingModel
{
	public int Id { get; set; }
	public string GivenName { get; set; } = string.Empty;
	public string? FamilyName { get; set; } = string.Empty;
    public string? LatestShard { get; set; } = string.Empty;

	public int? PlanetId { get; set; }

	public PlanetModel? Planet { get; set; }

	public DateTime? UpdatedOn { get; set; }
}
