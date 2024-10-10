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
		app.MapGet($"/Characters", HandleAsync)
			.WithTags(Tags.Characters);
	}

	[Authorize]
	[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
	protected virtual async Task<Results<Ok<IEnumerable<CharacterDto>>, ProblemHttpResult, NotFound>> HandleAsync(ICharacterService characterService)
	{
		try
		{
			var results = await characterService.GetCharacters();
			if (results != null && results.Any())
			{
				return TypedResults.Ok(results.Select(planet => planet.ToDto()));
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
