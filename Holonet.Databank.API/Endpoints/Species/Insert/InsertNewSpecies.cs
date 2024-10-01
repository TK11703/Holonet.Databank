using Holonet.Databank.Application.Services;
using Holonet.Databank.API.Filters;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Holonet.Databank.Core.Dtos;

namespace Holonet.Databank.API.Endpoints.Species.Insert;

public class InsertNewSpecies : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost($"/Species", HandleAsync)
			.AddEndpointFilter<ValidatorFilter<CreateSpeciesDto>>()
			.WithTags(Tags.Species);
	}

	protected virtual async Task<Results<Ok<int>, ProblemHttpResult>> HandleAsync(CreateSpeciesDto itemModel, ISpeciesService speciesService)
	{
		try
		{
			var newSpecies = new Core.Entities.Species
            {
				Name = itemModel.Name,
				Description = itemModel.Description,
				Shard = itemModel.Shard
			};
			int newId = await speciesService.CreateSpecies(newSpecies);
			return TypedResults.Ok(newId);
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
