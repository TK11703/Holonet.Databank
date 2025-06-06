﻿using Holonet.Databank.API.Filters;
using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Web.Resource;
namespace Holonet.Databank.API.Endpoints.HistoricalEvents.Update;

public class UpdateHistoricalEvent : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPut($"/HistoricalEvents/{{id}}", Handle)
			.AddEndpointFilter<ValidatorFilter<UpdateHistoricalEventDto>>()
			.WithTags(Tags.HistoricalEvents);
	}

	protected virtual async Task<Results<Ok<bool>, ProblemHttpResult>> Handle(int id, UpdateHistoricalEventDto itemModel, IHistoricalEventService historicalEventService, IAuthorService authorService)
	{
		try
		{
			var author = await authorService.GetAuthorByAzureId(itemModel.AzureId);
			if (author == null)
			{
				return TypedResults.Problem("Author not found");
			}
			var historicalEvent = new HistoricalEvent
			{
				Id = itemModel.Id,
				Name = itemModel.Name,
				DatePeriod = itemModel.DatePeriod,
				CharacterIds = itemModel.CharacterIds,
				PlanetIds = itemModel.PlanetIds,
				Aliases = itemModel.Aliases.Select(alias => new Alias { Name = alias, UpdatedBy = author }),
				UpdatedBy = author
			};
			var rowsUpdated = await historicalEventService.UpdateHistoricalEvent(historicalEvent);
			return TypedResults.Ok(rowsUpdated);
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
