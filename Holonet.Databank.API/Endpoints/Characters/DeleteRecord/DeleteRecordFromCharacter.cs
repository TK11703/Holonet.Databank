using Holonet.Databank.Application.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Holonet.Databank.API.Endpoints.Characters.DeleteRecord;

public class DeleteRecordFromCharacter : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapDelete($"/Characters/{{id}}/Record/{{recordId}}", Handle)
			.WithTags(Tags.Characters);
	}
	protected virtual async Task<Results<Ok<bool>, ProblemHttpResult>> Handle(int id, int recordId, IDataRecordService dataRecordService)
	{
		try
		{			
			var completed = await dataRecordService.DeleteDataRecord(recordId, characterId: id);
			return TypedResults.Ok(completed);
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
