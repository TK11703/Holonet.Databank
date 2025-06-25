using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Holonet.Databank.API.Endpoints.Planets.GetRecord;

public class GetPlanetRecord : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet($"/Planets/{{id}}/GetRecord/{{recordId}}", Handle)			
			.WithTags(Tags.Planets);
	}
	protected virtual async Task<Results<Ok<DataRecordDto>, ProblemHttpResult, NotFound>> Handle(int id, int recordId, IDataRecordService dataRecordService)
	{
		try
		{			
			var result = await dataRecordService.GetDataRecordById(recordId, planetId: id);
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
