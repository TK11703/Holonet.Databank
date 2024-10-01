using Holonet.Databank.API.Filters;
using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
namespace Holonet.Databank.API.Endpoints.Characters.Update;

public class UpdateCharacter : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPut($"/Characters/{{id}}", Handle)
			.AddEndpointFilter<ValidatorFilter<UpdateCharacterDto>>()
			.WithTags(Tags.Characters);
	}

	protected virtual async Task<Results<Ok<bool>, ProblemHttpResult>> Handle(int id, UpdateCharacterDto itemModel, ICharacterService characterService)
	{
		try
		{
			var character = new Character
			{
				Id = itemModel.Id,
				FirstName = itemModel.FirstName,
				LastName = itemModel.LastName,
				Description = itemModel.Description,
				Shard = itemModel.Shard,
				BirthDate = itemModel.BirthDate,
				PlanetId = itemModel.PlanetId
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
