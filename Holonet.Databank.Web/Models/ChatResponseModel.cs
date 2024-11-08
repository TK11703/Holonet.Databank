namespace Holonet.Databank.Web.Models;

public enum ChatResponseType
{
	User,
	Agent
}
public class ChatResponseModel
{
	public ChatResponseType Type { get; set; }

	public string Result { get; set; } = string.Empty;
}
