using Holonet.Databank.API.Configuration;
using Holonet.Databank.API.Filters;
using Holonet.Databank.Application.Services.AI;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Holonet.Databank.API.Endpoints.AIServices.Language.TextSummary;

public class ExecuteTextSummaryRequest : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost($"/AIServices/Language/TextSummary", HandleAsync)
            .AddEndpointFilter<ValidatorFilter<TextSummaryRequest>>()
            .WithTags(Tags.AIServices);
    }
    protected virtual async Task<Results<Ok<TextSummaryResult>, ProblemHttpResult>> HandleAsync(TextSummaryRequest request, [FromServices] IOptions<AppSettings> settings)
    {
        AppSettings appSettings = settings.Value;
        TextSummaryResult result = new TextSummaryResult();
        try
        {
            SummarizationService service = new SummarizationService(appSettings.AzureOpenAi.Model!, appSettings.AzureOpenAi.Endpoint!, appSettings.AzureOpenAi.ApiKey!);
            result.ResultText = await service.SummarizeContentAsync(request.Input);
            return TypedResults.Ok(result);
        }
        catch (Exception ex)
        {
            return TypedResults.Problem(ex.Message);
        }
    }
}
