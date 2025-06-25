using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Web.Models;

public class AuthorModel
{
	public int Id { get; set; }

	public Guid AzureId { get; set; }

	public string DisplayName { get; set; } = string.Empty;

	public string? Email { get; set; } = string.Empty;
}
