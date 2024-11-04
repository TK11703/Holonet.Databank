using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Holonet.Databank.API.Endpoints.Planets.GetRecords;

public class GetPlanetRecords : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet($"/Planets/{{id}}/GetRecords", Handle)			
			.WithTags(Tags.Planets);
	}
	protected virtual async Task<Results<Ok<IEnumerable<DataRecordDto>>, ProblemHttpResult>> Handle(int id, IDataRecordService dataRecordService)
	{
		try
		{			
			var results = await dataRecordService.GetDataRecordsById(planetId: id);
			if (results != null && results.Any())
			{
				return TypedResults.Ok(results.Select(record => record.ToDto()));
			}
			else
			{
				return TypedResults.Ok(Enumerable.Empty<DataRecordDto>());
			}
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
