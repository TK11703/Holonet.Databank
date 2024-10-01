using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Web.Models;

public class CharacterModel
{
	[Required]
	public int Id { get; set; }

	[Required]
	[StringLength(150)]
	public string FirstName { get; set; } = string.Empty;

	[Required]
	[StringLength(150)]
	public string LastName { get; set; } = string.Empty;

	[StringLength(150)]
	public string? Description { get; set; }

	[Url]
	[StringLength(500)]
	public string? Shard { get; set; }

	[StringLength(200)]
	public string? BirthDate { get; set; }

	public int? PlanetId { get; set; }

	public PlanetModel? Planet { get; set; }

    public IEnumerable<int> SpeciesIds { get; set; } = [];
    public IEnumerable<SpeciesModel> Species { get; set; } = [];


	[StringLength(250)]
	public string CreatedBy { get; set; } = string.Empty;

	[StringLength(250)]
	public string UpdatedBy { get; set; } = string.Empty;

	public DateTime CreatedOn { get; set; }
	public DateTime UpdatedOn { get; set; }
}
