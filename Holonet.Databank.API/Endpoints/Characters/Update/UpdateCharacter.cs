using Holonet.Databank.API.Filters;
using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Web.Resource;
namespace Holonet.Databank.API.Endpoints.Characters.Update;

public class UpdateCharacter : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPut($"/Characters/{{id}}", Handle)
			.AddEndpointFilter<ValidatorFilter<UpdateCharacterDto>>()
			.WithTags(Tags.Characters);
	}

	[Authorize]
	[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
	protected virtual async Task<Results<Ok<bool>, ProblemHttpResult>> Handle(int id, UpdateCharacterDto itemModel, ICharacterService characterService, IAuthorService authorService, IUserService userService)
	{
		try
		{
			var azureId = userService.GetAzureId();
			if (azureId == null)
			{
				return TypedResults.Problem("User not found");
			}
			var author = await authorService.GetAuthorByAzureId(azureId.Value);
			if (author == null)
			{
				return TypedResults.Problem("Author not found");
			}

			var character = new Character
			{
				Id = itemModel.Id,
				GivenName = itemModel.GivenName,
				FamilyName = itemModel.FamilyName,
				Description = itemModel.Description,
				Shard = itemModel.Shard,
				BirthDate = itemModel.BirthDate,
				PlanetId = itemModel.PlanetId,
				SpeciesIds = itemModel.SpeciesIds,
				UpdatedBy = author
			};
			var rowsUpdated = await characterService.UpdateCharacter(character);
			return TypedResults.Ok(rowsUpdated);
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
