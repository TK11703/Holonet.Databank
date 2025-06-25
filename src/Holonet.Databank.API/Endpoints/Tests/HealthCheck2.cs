using Microsoft.AspNetCore.Http.HttpResults;

namespace Holonet.Databank.API.Endpoints.Tests;

public class HealthCheck2 : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet($"/Tests/B", Handle)
			.WithTags(Tags.Tests);
	}

	protected virtual Results<Ok<string>, ProblemHttpResult, NotFound> Handle()
	{
		try
		{
			return TypedResults.Ok("Test B Success");
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}