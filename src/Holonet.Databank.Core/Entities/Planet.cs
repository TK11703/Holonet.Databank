
using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Entities;

public class Planet : EntityBase
{
	[Required]
	[StringLength(150)]
	public required string Name { get; set; }

	public IEnumerable<int> AliasIds { get; set; } = [];
	public IEnumerable<Alias> Aliases { get; set; } = [];

	public IEnumerable<int> DataRecordIds { get; set; } = [];
	public IEnumerable<DataRecord> DataRecords { get; set; } = [];
}
