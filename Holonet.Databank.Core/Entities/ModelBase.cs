
using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Entities;
public class EntityBase
{
	public int Id { get; set; }
	
	public DateTime CreatedOn { get; set; }

	[StringLength(250)]
	public string? CreatedBy { get; set; }

	public DateTime UpdatedOn { get; set; }

	[StringLength(250)]
	public string? UpdatedBy { get; set; }
}
