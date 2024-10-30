using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Web.Resource;

namespace Holonet.Databank.API.Endpoints.Planets.GetAll;

public class GetAllSpecies : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet($"/Species", HandleAsync)
			.WithTags(Tags.Species);
	}
	protected virtual async Task<Results<Ok<IEnumerable<SpeciesDto>>, ProblemHttpResult, NotFound>> HandleAsync(ISpeciesService speciesService)
	{
		try
		{
			var results = await speciesService.GetSpecies();
			if (results != null && results.Any())
			{
				return TypedResults.Ok(results.Select(species => species.ToDto()));
			}
			else
			{
				return TypedResults.Ok(Enumerable.Empty<SpeciesDto>());
			}
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
