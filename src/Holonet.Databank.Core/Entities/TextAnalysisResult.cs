
namespace Holonet.Databank.Core.Entities;
public class TextAnalysisResultBase
{
    public string? Input { get; set; }
    public string Sentiment { get; set; } = string.Empty;
    public double PositiveConfidenceScore { get; set; }
    public double NeutralConfidenceScore { get; set; }
    public double NegativeConfidenceScore { get; set; }
}

public class TextAnalysisResult : TextAnalysisResultBase
{
    public List<TextAnalysisResultBase> SentenceAnalysis { get; set; } = new List<TextAnalysisResultBase>();
}