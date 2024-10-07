using Holonet.Databank.API.Filters;
using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Web.Resource;

namespace Holonet.Databank.API.Endpoints.Characters.Exists;

public class CharacterExists : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost($"/Characters/Exists", HandleAsync)
			.AddEndpointFilter<ValidatorFilter<GetCharacterDto>>()
			.WithTags(Tags.Characters);
	}

	[Authorize]
	[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
	protected virtual async Task<Results<Ok<bool>, ProblemHttpResult>> HandleAsync(GetCharacterDto itemModel, ICharacterService characterService)
	{
		try
		{
			var exists = await characterService.CharacterExists(itemModel.Id, itemModel.FirstName, itemModel.LastName, itemModel.PlanetId);
			return TypedResults.Ok(exists);
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
