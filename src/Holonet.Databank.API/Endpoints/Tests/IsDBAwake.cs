using Holonet.Databank.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Holonet.Databank.API.Endpoints.Tests;

public class IsDBAwake : IEndpoint
{
	[Authorize]
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet($"/Tests/DBAwake", Handle)
			.WithTags(Tags.Tests);
	}
	protected virtual async Task<Results<Ok<string>, ProblemHttpResult>> Handle(IGenericDBService genericDBService)
	{
		try
		{
			var isDbAwake = await genericDBService.DBReady();
			return isDbAwake ? TypedResults.Ok("Database is available!") : TypedResults.Problem("Database not awake");
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}