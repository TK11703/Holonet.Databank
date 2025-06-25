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
		app.MapGet($"/Planets", HandleGetAsync)
			.WithTags(Tags.Planets);

        app.MapPost($"/Planets/GetAll", HandlePostAsync)
            .WithTags(Tags.Planets);
    }

	protected virtual async Task<Results<Ok<IEnumerable<PlanetDto>>, ProblemHttpResult, NotFound>> HandleGetAsync(IPlanetService planetService)
	{
		try
		{
			var results = await planetService.GetPlanets(true, true);
			if (results != null && results.Any())
			{
				return TypedResults.Ok(results.Select(result => result.ToDto()));
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

    protected virtual async Task<Results<Ok<IEnumerable<PlanetDto>>, ProblemHttpResult, NotFound>> HandlePostAsync(GetEntityCollectionDto postData, IPlanetService planetService)
    {
        try
        {
            var results = await planetService.GetPlanets(postData.PopulateEntities, postData.PopulateDataRecords);
            if (results != null && results.Any())
            {
                return TypedResults.Ok(results.Select(result => result.ToDto()));
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
