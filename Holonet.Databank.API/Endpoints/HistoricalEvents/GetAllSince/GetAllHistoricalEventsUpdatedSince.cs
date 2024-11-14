using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Holonet.Databank.API.Endpoints.HistoricalEvents.GetAllSince;

public class GetAllHistoricalEventsUpdatedSince : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet($"/HistoricalEvents/UpdatedSince/{{utcTicks}}", HandleGetAsync)
			.WithTags(Tags.HistoricalEvents);

        app.MapPost($"/HistoricalEvents/UpdatedSince", HandlePostAsync)
            .WithTags(Tags.HistoricalEvents);
    }
	protected virtual async Task<Results<Ok<IEnumerable<HistoricalEventDto>>, ProblemHttpResult, NotFound>> HandleGetAsync(long utcTicks, IHistoricalEventService historicalEventService)
	{
		try
		{
			var results = await historicalEventService.GetHistoricalEvents(utcTicks, true, true);
			if (results != null && results.Any())
			{
				return TypedResults.Ok(results.Select(result => result.ToDto()));
			}
			else
			{
				return TypedResults.Ok(Enumerable.Empty<HistoricalEventDto>());
			}
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}

    protected virtual async Task<Results<Ok<IEnumerable<HistoricalEventDto>>, ProblemHttpResult, NotFound>> HandlePostAsync(GetEntityCollectionDto postData, IHistoricalEventService historicalEventService)
    {
        try
        {
            if (postData.UtcTicks.HasValue)
            {
                var results = await historicalEventService.GetHistoricalEvents(postData.UtcTicks.Value, postData.PopulateEntities, postData.PopulateDataRecords);
                if (results != null && results.Any())
                {
                    return TypedResults.Ok(results.Select(result => result.ToDto()));
                }
                else
                {
                    return TypedResults.Ok(Enumerable.Empty<HistoricalEventDto>());
                }
            }
            else
            {
                return TypedResults.Problem("UtcTicks value is required.");
            }
        }
        catch (Exception ex)
        {
            return TypedResults.Problem(ex.Message);
        }
    }
}
