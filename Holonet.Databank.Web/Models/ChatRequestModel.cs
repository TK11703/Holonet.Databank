namespace Holonet.Databank.Web.Models;

public class ChatRequestModel
{
	public Guid AzureId { get; set; }

	public string Prompt { get; set; } = string.Empty;
}
