using Holonet.Databank.API.Filters;
using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Web.Resource;

namespace Holonet.Databank.API.Endpoints.Characters.Insert;

public class InsertNewCharacter : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost($"/Characters", HandleAsync)
			.AddEndpointFilter<ValidatorFilter<CreateCharacterDto>>()
			.WithTags(Tags.Characters);
	}

	[Authorize]
	[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
	protected virtual async Task<Results<Ok<int>, ProblemHttpResult>> HandleAsync(CreateCharacterDto itemModel, ICharacterService characterService, IAuthorService authorService, IUserService userService)
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
				GivenName = itemModel.GivenName,
				FamilyName = itemModel.FamilyName,
				Description = itemModel.Description,
				Shard = itemModel.Shard,
				BirthDate = itemModel.BirthDate,
				PlanetId = itemModel.PlanetId,
				SpeciesIds = itemModel.SpeciesIds,
				UpdatedBy = author
			};

			int newId = await characterService.CreateCharacter(character);
			return TypedResults.Ok(newId);
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
