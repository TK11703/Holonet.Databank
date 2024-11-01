
using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Entities;

public class DataRecord : EntityBase
{
	[Required]	
	public required string Data { get; set; }

	public int? CharacterId { get; set; }

	public int? PlanetId { get; set; }

	public int? SpeciesId { get; set; }

	public int? HistoricalEventId { get; set; }

}
