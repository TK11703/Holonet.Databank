
using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Entities;

public class Alias : EntityBase
{
	[Required]
	[StringLength(150)]
	public required string Name { get; set; }

	public int? CharacterId { get; set; }

	public int? PlanetId { get; set; }

	public int? SpeciesId { get; set; }

}
