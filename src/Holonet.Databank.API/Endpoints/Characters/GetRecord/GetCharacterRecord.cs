using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Holonet.Databank.API.Endpoints.Characters.GetRecord;

public class GetCharacterRecord : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet($"/Characters/{{id}}/GetRecord/{{recordId}}", Handle)			
			.WithTags(Tags.Characters);
	}
	protected virtual async Task<Results<Ok<DataRecordDto>, ProblemHttpResult, NotFound>> Handle(int id, int recordId, IDataRecordService dataRecordService)
	{
		try
		{			
			var result = await dataRecordService.GetDataRecordById(recordId, characterId: id);
			if (result != null)
			{
				return TypedResults.Ok(result.ToDto());
			}
			else
			{
				return TypedResults.NotFound();
            }
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
