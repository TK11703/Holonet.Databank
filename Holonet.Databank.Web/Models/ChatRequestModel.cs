using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Web.Models;

public class ChatRequestModel
{
    public Guid AzureId { get; set; }

	[Required(ErrorMessage ="Please enter your message to the agent here.")]
	public string Prompt { get; set; } = string.Empty;
}
