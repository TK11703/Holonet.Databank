using Microsoft.SemanticKernel;

namespace Holonet.Databank.Application.Services.AI;
public class SummarizationService : ISummarizationService
{
    private readonly Kernel _kernel;

    public SummarizationService(string deploymentName, string endpoint, string apiKey)
    {
        var builder = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(deploymentName: deploymentName, endpoint: endpoint, apiKey: apiKey);
        _kernel = builder.Build();
    }
    public async Task<string> SummarizeContentAsync(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            throw new ArgumentException("Content cannot be null or empty.", nameof(content));
        }
        // Create a prompt for summarization
        var promptTemplate = @"
        Summarize the following content into key points. 
        Focus on the main ideas and important details.
        Content: {{$input}}
        Summary:";

        // Create function for summarization
        var summarizeFunction = _kernel.CreateFunctionFromPrompt(promptTemplate);

        // Get the summary
        var result = await _kernel.InvokeAsync(summarizeFunction, new() { ["input"] = content });

        return result?.GetValue<string>() ?? string.Empty;
    }

}
