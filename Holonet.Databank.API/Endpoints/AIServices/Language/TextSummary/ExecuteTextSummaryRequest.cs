using Holonet.Databank.API.Filters;
using Holonet.Databank.Application.Services.AI;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Holonet.Databank.API.Endpoints.AIServices.Language.TextSummary;

public class ExecuteTextSummaryRequest : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost($"/AIServices/Language/TextSummary", HandleAsync)
            .AddEndpointFilter<ValidatorFilter<TextSummaryRequest>>()
            .WithTags(Tags.AIServices);
    }
    protected virtual async Task<Results<Ok<TextSummaryResult>, ProblemHttpResult>> HandleAsync(TextSummaryRequest request, IConfiguration config)
    {
        TextSummaryResult result = new TextSummaryResult();
        try
        {
            SummarizationService service = new SummarizationService(config.GetValue<string>("AzureOpenAi:Model")!, config.GetValue<string>("AzureOpenAi:Endpoint")!, config.GetValue<string>("AzureOpenAi:ApiKey")!);
            result.ResultText = await service.SummarizeContentAsync(request.Input);
            //TextAnalysisService service = new TextAnalysisService(config.GetValue<string>("AzureAiLanguage:ApiKey")!, config.GetValue<string>("AzureAiLanguage:Endpoint")!);
            //result.ResultText = await service.GetSummaryAbstract(request.Input);
            return TypedResults.Ok(result);
        }
        catch (Exception ex)
        {
            return TypedResults.Problem(ex.Message);
        }
    }
}
