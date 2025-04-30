
using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Entities;

public class Character : EntityBase
{
	[Required]
	[StringLength(150)]
	public required string GivenName { get; set; }
	
	[StringLength(150)]
	public string? FamilyName { get; set; }	

	[StringLength(200)]
	public string? BirthDate { get; set; }

	public int? PlanetId { get; set; }

	public Planet? Planet { get; set; }

    public IEnumerable<int> SpeciesIds { get; set; } = [];
    public IEnumerable<Species> Species { get; set; } = [];

	public IEnumerable<int> AliasIds { get; set; } = [];
	public IEnumerable<Alias> Aliases { get; set; } = [];

	public IEnumerable<int> DataRecordIds { get; set; } = [];
	public IEnumerable<DataRecord> DataRecords { get; set; } = [];
}

