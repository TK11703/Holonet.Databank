using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Web.Resource;

namespace Holonet.Databank.API.Endpoints.Characters.GetAll;

public class GetAllCharacters : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet($"/Characters", HandleGetAsync)
			.WithTags(Tags.Characters);

        app.MapPost($"/Characters/GetAll", HandlePostAsync)
            .WithTags(Tags.Characters);
    }
	protected virtual async Task<Results<Ok<IEnumerable<CharacterDto>>, ProblemHttpResult, NotFound>> HandleGetAsync(ICharacterService characterService)
	{
		try
		{
			var results = await characterService.GetCharacterList(true, true);
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
            var results = await characterService.GetCharacterList(postData.PopulateEntities, postData.PopulateDataRecords);
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
}
