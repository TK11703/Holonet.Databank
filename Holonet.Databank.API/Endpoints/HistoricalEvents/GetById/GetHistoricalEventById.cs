using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Web.Resource;

namespace Holonet.Databank.API.Endpoints.HistoricalEvents.GetById;

public class GetHistoricalEventById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet($"/HistoricalEvents/{{id}}", HandleAsync)
			.WithTags(Tags.HistoricalEvents);
	}

	[Authorize]
	[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
	protected virtual async Task<Results<Ok<HistoricalEventDto>, ProblemHttpResult, NotFound>> HandleAsync(int id, IHistoricalEventService historicalEventService)
	{
		try
		{
			var result = await historicalEventService.GetHistoricalEventById(id);
			if (result == null)
				return TypedResults.NotFound();
			return TypedResults.Ok(result.ToDto());
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
