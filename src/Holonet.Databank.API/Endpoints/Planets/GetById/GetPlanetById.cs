using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Web.Resource;

namespace Holonet.Databank.API.Endpoints.Planets.GetById;

public class GetPlanetById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet($"/Planets/{{id}}", HandleAsync)
			.WithTags(Tags.Planets);
	}

	protected virtual async Task<Results<Ok<PlanetDto>, ProblemHttpResult, NotFound>> HandleAsync(int id, IPlanetService planetService)
	{
		try
		{
			var result = await planetService.GetPlanetById(id);
			if (result == null)
				return TypedResults.NotFound();
			return TypedResults.Ok(result.ToDto());
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
