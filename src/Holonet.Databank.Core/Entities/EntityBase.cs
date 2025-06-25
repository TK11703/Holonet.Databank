
using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Entities;
public class EntityBase
{
	public int Id { get; set; }
	
	public DateTime? UpdatedOn { get; set; }

	public int AuthorId { get; set; }

	public required Author UpdatedBy { get; set; }
}
