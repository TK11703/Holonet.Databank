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
		app.MapGet($"/Species", HandleGetAsync)
			.WithTags(Tags.Species);

        app.MapPost($"/Species/GetAll", HandlePostAsync)
            .WithTags(Tags.Species);
    }
	protected virtual async Task<Results<Ok<IEnumerable<SpeciesDto>>, ProblemHttpResult, NotFound>> HandleGetAsync(ISpeciesService speciesService)
	{
		try
		{
			var results = await speciesService.GetSpecies(true, true);
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
            var results = await speciesService.GetSpecies(postData.PopulateEntities, postData.PopulateDataRecords);
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
}
