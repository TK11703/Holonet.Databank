using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Web.Resource;

namespace Holonet.Databank.API.Endpoints.Planets.GetAll;

public class GetAllPlanets : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet($"/Planets", HandleAsync)
			.WithTags(Tags.Planets);
	}

	protected virtual async Task<Results<Ok<IEnumerable<PlanetDto>>, ProblemHttpResult, NotFound>> HandleAsync(IPlanetService planetService)
	{
		try
		{
			var results = await planetService.GetPlanets();
			if (results != null && results.Any())
			{
				return TypedResults.Ok(results.Select(planet => planet.ToDto()));
			}
			else
			{
				return TypedResults.Ok(Enumerable.Empty<PlanetDto>());
			}
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
