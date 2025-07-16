
namespace Holonet.Databank.AppFunctions.Configuration;
internal static class MessageConstants
{   
    public const string HtmlHarvesterNoData = "The HtmlHarvester returned no data chunks to process.";
    public const string HtmlHarvesterError = "The HtmlHarvester encountered an error while processing the shard.";

    public const string TextSummarizationNoData = "The TextSummarization did not return any resulting text as a summary.";
    public const string TextSummarizationError = "The TextSummarization encountered an error while processing the html chunks.";
}
