using Holonet.Databank.API.Filters;
using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Web.Resource;

namespace Holonet.Databank.API.Endpoints.HistoricalEvents.Exists;

public class HistoricalEventExists : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost($"/HistoricalEvents/Exists", HandleAsync)
			.AddEndpointFilter<ValidatorFilter<GetHistoricalEventDto>>()
			.WithTags(Tags.HistoricalEvents);
	}
	protected virtual async Task<Results<Ok<bool>, ProblemHttpResult>> HandleAsync(GetHistoricalEventDto itemModel, IHistoricalEventService historicalEventService)
	{
		try
		{
			var exists = await historicalEventService.HistoricalEventExists(itemModel.Id, itemModel.Name);
			return TypedResults.Ok(exists);
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
