using Holonet.Databank.API.Filters;
using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
namespace Holonet.Databank.API.Endpoints.HistoricalEvents.Update;

public class UpdateHistoricalEvent : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPut($"/HistoricalEvents/{{id}}", Handle)
			.AddEndpointFilter<ValidatorFilter<UpdateHistoricalEventDto>>()
			.WithTags(Tags.HistoricalEvents);
	}

	protected virtual async Task<Results<Ok<bool>, ProblemHttpResult>> Handle(int id, UpdateHistoricalEventDto itemModel, IHistoricalEventService historicalEventService)
	{
		try
		{
			var historicalEvent = new HistoricalEvent
			{
				Id = itemModel.Id,
				Name = itemModel.Name,
				Description = itemModel.Description,
				DatePeriod = itemModel.DatePeriod,
				Shard = itemModel.Shard,
				CharacterIds = itemModel.CharacterIds,
				PlanetIds = itemModel.PlanetIds
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
