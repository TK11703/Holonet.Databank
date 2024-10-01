using Holonet.Databank.Application.Services;
using Holonet.Databank.API.Filters;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Holonet.Databank.Core.Dtos;

namespace Holonet.Databank.API.Endpoints.Planets.Insert;

public class InsertNewPlanet : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost($"/Planets", HandleAsync)
			.AddEndpointFilter<ValidatorFilter<CreatePlanetDto>>()
			.WithTags(Tags.Planets);
	}

	protected virtual async Task<Results<Ok<int>, ProblemHttpResult>> HandleAsync(CreatePlanetDto itemModel, IPlanetService planetService)
	{
		try
		{
			var newPlanet = new Planet
			{
				Name = itemModel.Name,
				Description = itemModel.Description,
				Shard = itemModel.Shard
			};
			int newId = await planetService.CreatePlanet(newPlanet);
			return TypedResults.Ok(newId);
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
