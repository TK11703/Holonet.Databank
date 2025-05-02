
namespace Holonet.Databank.Application.Services.AI;

public interface ISummarizationService
{
    Task<string> SummarizeContentAsync(string content);
}