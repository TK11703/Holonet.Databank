using Holonet.Databank.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Web.Resource;

namespace Holonet.Databank.API.Endpoints.Characters.Delete;

public class DeleteCharacterById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapDelete($"/Characters/{{id}}", HandleAsync)
			.WithTags(Tags.Characters);
	}

	[Authorize]
	[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
	protected virtual async Task<Results<Ok<bool>, ProblemHttpResult>> HandleAsync(int id, ICharacterService characterService)
	{
		try
		{
			var completed = await characterService.DeleteCharacter(id);
			return TypedResults.Ok(completed);
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}