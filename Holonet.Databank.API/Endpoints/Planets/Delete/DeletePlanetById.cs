using Holonet.Databank.Application.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Holonet.Databank.API.Endpoints.Planets.Delete;

public class DeletePlanetById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapDelete($"/Planets/{{id}}", HandleAsync)
			.WithTags(Tags.Planets);
	}

	[Authorize]
	[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
	protected virtual async Task<Results<Ok<bool>, ProblemHttpResult>> HandleAsync(int id, IPlanetService planetService)
	{
		try
		{
			var completed = await planetService.DeletePlanet(id);
			return TypedResults.Ok(completed);
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}