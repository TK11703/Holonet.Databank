
using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Entities;

public class Author 
{
	public int Id { get; set; }

	[Required]
	public required Guid AzureId { get; set; }

	[Required]
	[StringLength(255)]
	public required string DisplayName { get; set; }

	[EmailAddress]
	[StringLength(255)]
	public string? Email { get; set; }
	
}
