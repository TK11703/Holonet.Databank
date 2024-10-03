using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Web.Resource;

namespace Holonet.Databank.API.Endpoints.Characters.GetById;

public class GetCharacterById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet($"/Characters/{{id}}", HandleAsync)
			.WithTags(Tags.Characters);
	}

	[Authorize]
	[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
	protected virtual async Task<Results<Ok<CharacterDto>, ProblemHttpResult, NotFound>> HandleAsync(int id, ICharacterService characterService)
	{
		try
		{
			var result = await characterService.GetCharacterById(id);
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
