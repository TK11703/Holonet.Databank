using Holonet.Databank.Application.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Holonet.Databank.API.Endpoints.Planets.DeleteRecord;

public class DeleteRecordFromPlanet : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapDelete($"/Planets/{{id}}/Record/{{recordId}}", Handle)
			.WithTags(Tags.Planets);
	}
	protected virtual async Task<Results<Ok<bool>, ProblemHttpResult>> Handle(int id, int recordId, IDataRecordService dataRecordService)
	{
		try
		{			
			var completed = await dataRecordService.DeleteDataRecord(recordId, planetId: id);
			return TypedResults.Ok(completed);
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
