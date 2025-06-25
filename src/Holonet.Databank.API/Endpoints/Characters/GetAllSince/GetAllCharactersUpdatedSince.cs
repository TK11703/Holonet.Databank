using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Holonet.Databank.API.Endpoints.Characters.GetAllSince;

public class GetAllCharactersUpdatedSince : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet($"/Characters/UpdatedSince/{{utcTicks}}", HandleGetAsync)
			.WithTags(Tags.Characters);

        app.MapPost($"/Characters/UpdatedSince", HandlePostAsync)
            .WithTags(Tags.Characters);
    }
	protected virtual async Task<Results<Ok<IEnumerable<CharacterDto>>, ProblemHttpResult, NotFound>> HandleGetAsync(long utcTicks, ICharacterService characterService)
	{
		try
		{
			var results = await characterService.GetCharacterList(utcTicks, true, true);
			if (results != null && results.Any())
			{
				return TypedResults.Ok(results.Select(result => result.ToDto()));
			}
			else
			{
				return TypedResults.Ok(Enumerable.Empty<CharacterDto>());
			}
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}

    protected virtual async Task<Results<Ok<IEnumerable<CharacterDto>>, ProblemHttpResult, NotFound>> HandlePostAsync(GetEntityCollectionDto postData, ICharacterService characterService)
    {
        try
        {
            if (postData.UtcTicks.HasValue)
            {
                var results = await characterService.GetCharacterList(postData.UtcTicks.Value, postData.PopulateEntities, postData.PopulateDataRecords);
                if (results != null && results.Any())
                {
                    return TypedResults.Ok(results.Select(result => result.ToDto()));
                }
                else
                {
                    return TypedResults.Ok(Enumerable.Empty<CharacterDto>());
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
