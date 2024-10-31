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

	protected virtual async Task<Results<Ok<int>, ProblemHttpResult>> HandleAsync(CreateCharacterDto itemModel, ICharacterService characterService, IAuthorService authorService)
	{
		try
		{
			var author = await authorService.GetAuthorByAzureId(itemModel.AzureId);
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
				Aliases = itemModel.Aliases.Select(alias => new Alias { Name = alias, UpdatedBy = author }),
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
