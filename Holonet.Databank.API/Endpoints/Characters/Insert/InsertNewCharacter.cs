using Holonet.Databank.API.Filters;
using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Holonet.Databank.API.Endpoints.Characters.Insert;

public class InsertNewCharacter : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost($"/Characters", HandleAsync)
			.AddEndpointFilter<ValidatorFilter<CreateCharacterDto>>()
			.WithTags(Tags.Characters);
	}

	protected virtual async Task<Results<Ok<int>, ProblemHttpResult>> HandleAsync(CreateCharacterDto itemModel, ICharacterService characterService)
	{
		try
		{
			var character = new Character
			{
				FirstName = itemModel.FirstName,
				LastName = itemModel.LastName,
				Description = itemModel.Description,
				Shard = itemModel.Shard,
				BirthDate = itemModel.BirthDate,
				PlanetId = itemModel.PlanetId
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
