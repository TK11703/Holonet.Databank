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
	protected virtual async Task<Results<Ok<bool>, ProblemHttpResult>> Handle(int id, UpdateCharacterDto itemModel, ICharacterService characterService, IAuthorService authorService)
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
				Id = itemModel.Id,
				GivenName = itemModel.GivenName,
				FamilyName = itemModel.FamilyName,
				Shard = itemModel.Shard,
				BirthDate = itemModel.BirthDate,
				PlanetId = itemModel.PlanetId,
				SpeciesIds = itemModel.SpeciesIds,
				Aliases = itemModel.Aliases.Select(alias => new Alias { Name = alias, UpdatedBy = author }),
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
