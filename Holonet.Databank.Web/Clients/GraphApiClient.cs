using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace Holonet.Databank.Web.Clients;

public class GraphApiClient(GraphServiceClient graphServiceClient)
{
	private readonly GraphServiceClient _graphServiceClient = graphServiceClient;

	public async Task<string> GetGraphApiUserId()
    {
        var user = await _graphServiceClient.Me.GetAsync(requestConfiguration =>
        {
            requestConfiguration.QueryParameters.Select = ["id"];
        });
        if(user == null)
			return string.Empty;
        else
			return string.IsNullOrEmpty(user.Id) ? string.Empty : user.Id;
    }

    public async Task<User?> GetGraphApiUser()
    {
        return await _graphServiceClient.Me.GetAsync(requestConfiguration =>
        {
            requestConfiguration.QueryParameters.Select = ["id", "displayName", "mail", "userPrincipalName", "mobilePhone" ];
        });
    }

    public async Task<User?> GetGraphApiUser(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return null;
        return await _graphServiceClient.Users[userId].GetAsync(requestConfiguration =>
        {
            requestConfiguration.QueryParameters.Select = ["id", "displayName", "mail", "userPrincipalName", "mobilePhone"];
        });
    }

	public async Task<string> GetGraphApiProfilePhoto()
	{
		var photo = string.Empty;
		// Get user photo
		using (var photoStream = await _graphServiceClient.Me.Photo.Content.GetAsync().ConfigureAwait(false))
		{
            if (photoStream != null)
            {
                byte[] photoByte = ((MemoryStream)photoStream).ToArray();
                photo = Convert.ToBase64String(photoByte);
            }
		}

		return photo;
	}
}
