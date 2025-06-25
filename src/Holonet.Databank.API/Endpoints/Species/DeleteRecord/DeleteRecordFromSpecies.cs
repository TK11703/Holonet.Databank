using Holonet.Databank.Application.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Holonet.Databank.API.Endpoints.Species.DeleteRecord;

public class DeleteRecordFromSpecies : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapDelete($"/Species/{{id}}/Record/{{recordId}}", Handle)
			.WithTags(Tags.Species);
	}
	protected virtual async Task<Results<Ok<bool>, ProblemHttpResult>> Handle(int id, int recordId, IDataRecordService dataRecordService)
	{
		try
		{			
			var completed = await dataRecordService.DeleteDataRecord(recordId, speciesId: id);
			return TypedResults.Ok(completed);
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
