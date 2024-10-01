﻿using Holonet.Databank.API.Filters;
using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
namespace Holonet.Databank.API.Endpoints.Planets.Update;

public class UpdatePlanet : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPut($"/Planets/{{id}}", Handle)
			.AddEndpointFilter<ValidatorFilter<UpdatePlanetDto>>()
			.WithTags(Tags.Planets);
	}

	protected virtual async Task<Results<Ok<bool>, ProblemHttpResult>> Handle(int id, UpdatePlanetDto itemModel, IPlanetService planetService)
	{
		try
		{
			var planet = new Planet
			{
				Id = itemModel.Id,
				Name = itemModel.Name,
				Description = itemModel.Description,
				Shard = itemModel.Shard
			};
			var rowsUpdated = await planetService.UpdatePlanet(planet);
			return TypedResults.Ok(rowsUpdated);
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}