using Holonet.Databank.Application.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Holonet.Databank.API.Endpoints.HistoricalEvents.DeleteRecord;

public class DeleteRecordFromHistoricalEvent : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapDelete($"/HistoricalEvents/{{id}}/Record/{{recordId}}", Handle)
			.WithTags(Tags.HistoricalEvents);
	}
	protected virtual async Task<Results<Ok<bool>, ProblemHttpResult>> Handle(int id, int recordId, IDataRecordService dataRecordService)
	{
		try
		{			
			var completed = await dataRecordService.DeleteDataRecord(recordId, historicalEventId: id);
			return TypedResults.Ok(completed);
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
