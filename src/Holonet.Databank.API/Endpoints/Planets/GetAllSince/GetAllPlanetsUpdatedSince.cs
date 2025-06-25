using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Holonet.Databank.API.Endpoints.Planets.GetAllSince;

public class GetAllPlanetsUpdatedSince : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet($"/Planets/UpdatedSince/{{utcTicks}}", HandleGetAsync)
			.WithTags(Tags.Planets);

        app.MapPost($"/Planets/UpdatedSince/", HandlePostAsync)
            .WithTags(Tags.Planets);
    }
	protected virtual async Task<Results<Ok<IEnumerable<PlanetDto>>, ProblemHttpResult, NotFound>> HandleGetAsync(long utcTicks, IPlanetService planetService)
	{
		try
		{
			var results = await planetService.GetPlanets(utcTicks, true, true);
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
            if (postData.UtcTicks.HasValue)
            {
                var results = await planetService.GetPlanets(postData.UtcTicks.Value, postData.PopulateEntities, postData.PopulateDataRecords);
                if (results != null && results.Any())
                {
                    return TypedResults.Ok(results.Select(result => result.ToDto()));
                }
                else
                {
                    return TypedResults.Ok(Enumerable.Empty<PlanetDto>());
                }
            }
            else
            {
                return TypedResults.Problem("UtcTicks value is null.");
            }
        }
        catch (Exception ex)
        {
            return TypedResults.Problem(ex.Message);
        }
    }
}
