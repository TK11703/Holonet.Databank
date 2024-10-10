using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Web.Models;

public class CharacterModel
{
	public int Id { get; set; }

	[Required]
	[StringLength(150)]
	public string FirstName { get; set; } = string.Empty;

	[Required]
	[StringLength(150)]
	public string LastName { get; set; } = string.Empty;

	public string? Description { get; set; }

	[Url]
	[StringLength(500)]
	public string? Shard { get; set; }

	[StringLength(200)]
	public string? BirthDate { get; set; }

	public int? PlanetId { get; set; }

	public PlanetModel? Planet { get; set; }

    public List<int> SpeciesIds { get; set; } = [];
    public IEnumerable<SpeciesModel> Species { get; set; } = [];

	public AuthorModel? UpdatedBy { get; set; }

	public DateTime? UpdatedOn { get; set; }
}
