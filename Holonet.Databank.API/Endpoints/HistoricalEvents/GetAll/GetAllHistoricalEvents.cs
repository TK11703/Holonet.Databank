using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Holonet.Databank.API.Endpoints.HistoricalEvents.GetAll;

public class GetAllHistoricalEvents : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet($"/HistoricalEvents", HandleGetAsync)
			.WithTags(Tags.HistoricalEvents);

        app.MapPost($"/HistoricalEvents/GetAll", HandlePostAsync)
            .WithTags(Tags.HistoricalEvents);
    }
	protected virtual async Task<Results<Ok<IEnumerable<HistoricalEventDto>>, ProblemHttpResult, NotFound>> HandleGetAsync(IHistoricalEventService historicalEventService)
	{
		try
		{
			var results = await historicalEventService.GetHistoricalEvents(true, true);
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
            var results = await historicalEventService.GetHistoricalEvents(postData.PopulateEntities, postData.PopulateDataRecords);
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
}
