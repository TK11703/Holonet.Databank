using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Holonet.Databank.API.Endpoints.Species.GetAllSince;

public class GetAllSpeciesUpdatedSince : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet($"/Species/UpdatedSince/{{utcTicks}}", HandleGetAsync)
			.WithTags(Tags.Species);

        app.MapPost($"/Species/UpdatedSince", HandlePostAsync)
            .WithTags(Tags.Species);
    }
	protected virtual async Task<Results<Ok<IEnumerable<SpeciesDto>>, ProblemHttpResult, NotFound>> HandleGetAsync(long utcTicks, ISpeciesService speciesService)
	{
		try
		{
			var results = await speciesService.GetSpecies(utcTicks, true, true);
			if (results != null && results.Any())
			{
				return TypedResults.Ok(results.Select(result => result.ToDto()));
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

    protected virtual async Task<Results<Ok<IEnumerable<SpeciesDto>>, ProblemHttpResult, NotFound>> HandlePostAsync(GetEntityCollectionDto postData, ISpeciesService speciesService)
    {
        try
        {
            if (postData.UtcTicks.HasValue)
            {
                var results = await speciesService.GetSpecies(postData.UtcTicks.Value, postData.PopulateEntities, postData.PopulateDataRecords);
                if (results != null && results.Any())
                {
                    return TypedResults.Ok(results.Select(result => result.ToDto()));
                }
                else
                {
                    return TypedResults.Ok(Enumerable.Empty<SpeciesDto>());
                }
            }
            else
            {
                return TypedResults.Problem("UtcTicks value is required.");
            }
        }
        catch (Exception ex)
        {
            return TypedResults.Problem(ex.Message);
        }
    }
}
