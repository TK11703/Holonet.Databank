using Holonet.Databank.API.Filters;
using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Web.Resource;

namespace Holonet.Databank.API.Endpoints.Planets.Exists;

public class PlanetExists : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost($"/Planets/Exists", HandleAsync)
			.AddEndpointFilter<ValidatorFilter<GetPlanetDto>>()
			.WithTags(Tags.Planets);
	}

	[Authorize]
	[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
	protected virtual async Task<Results<Ok<bool>, ProblemHttpResult>> HandleAsync(GetPlanetDto itemModel, IPlanetService planetService)
	{
		try
		{
			var exists = await planetService.PlanetExists(itemModel.Id, itemModel.Name);
			return TypedResults.Ok(exists);
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
