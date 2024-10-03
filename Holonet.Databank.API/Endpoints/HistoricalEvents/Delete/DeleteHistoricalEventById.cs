using Holonet.Databank.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Web.Resource;

namespace Holonet.Databank.API.Endpoints.HistoricalEvents.Delete;

public class DeleteHistoricalEventById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapDelete($"/HistoricalEvents/{{id}}", HandleAsync)
			.WithTags(Tags.HistoricalEvents);
	}

	[Authorize]
	[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
	protected virtual async Task<Results<Ok<bool>, ProblemHttpResult>> HandleAsync(int id, IHistoricalEventService historicalEventService)
	{
		try
		{
			var completed = await historicalEventService.DeleteHistoricalEvent(id);
			return TypedResults.Ok(completed);
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}