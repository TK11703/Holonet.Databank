
using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Entities;

public class HistoricalEvent : EntityBase
{
	[Required]
	[StringLength(150)]
	public required string Name { get; set; }

	[StringLength(200)]
	public string? DatePeriod { get; set; }

	[Url]
	[StringLength(500)]
	public string? Shard { get; set; }

	public IEnumerable<int> CharacterIds { get; set; } = [];
	public IEnumerable<Character> Characters { get; set; } = [];

	public IEnumerable<int> PlanetIds { get; set; } = [];
	public IEnumerable<Planet> Planets { get; set; } = [];

	public IEnumerable<int> AliasIds { get; set; } = [];
	public IEnumerable<Alias> Aliases { get; set; } = [];

	public IEnumerable<int> DataRecordIds { get; set; } = [];
	public IEnumerable<DataRecord> DataRecords { get; set; } = [];

}
