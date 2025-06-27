using Holonet.Databank.Application.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Holonet.Databank.API.Endpoints.Species.RecordExists;

public class SpeciesRecordExists : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet($"/Species/{{id}}/Record", HandleAsync)
            .WithTags(Tags.Species);
    }
    protected virtual async Task<Results<Ok<bool>, ProblemHttpResult>> HandleAsync(int id, [FromQuery] string? shard, IDataRecordService dataRecordService)
    {
        if (string.IsNullOrWhiteSpace(shard))
            return TypedResults.Problem("Shard cannot be null, empty, or whitespaces .", statusCode: StatusCodes.Status400BadRequest);

        try
        {
            var exists = await dataRecordService.RecordExists(shard, speciesId: id);
            return TypedResults.Ok(exists);
        }
        catch (Exception ex)
        {
            return TypedResults.Problem(title: "An unexpected error occurred.", detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}