using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Web.Resource;

namespace Holonet.Databank.API.Endpoints.Species.GetById;

public class GetSpeciesById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet($"/Species/{{id}}", HandleAsync)
			.WithTags(Tags.Species);
	}
	protected virtual async Task<Results<Ok<SpeciesDto>, ProblemHttpResult, NotFound>> HandleAsync(int id, ISpeciesService speciesService)
	{
		try
		{
			var result = await speciesService.GetSpeciesById(id);
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
