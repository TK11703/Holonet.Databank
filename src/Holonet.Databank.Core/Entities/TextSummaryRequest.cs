namespace Holonet.Databank.Core.Entities;

public class TextSummaryRequest
{
    public string Input { get; set; } = string.Empty;
    public string? SourceLangCode { get; set; }
    public string TargetLangCode { get; set; } = string.Empty;
    public bool Language { get; set; } = false;
    public bool Sentiment { get; set; } = false;
    public bool KeyPhrases { get; set; } = false;
    public bool Entities { get; set; } = false;
    public bool PiiEntites { get; set; } = false;
    public bool LinkedEntities { get; set; } = false;
    public bool NamedEntityRecognition { get; set; } = false;
    public bool Summary { get; set; } = false;
    public bool AbstractiveSummary { get; set; } = false;
}
