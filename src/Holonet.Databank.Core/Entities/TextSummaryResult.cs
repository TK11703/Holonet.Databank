

namespace Holonet.Databank.Core.Entities;
public class TextSummaryResult
{
    public string? SourceLangCode { get; set; }
    public string? SourceScriptCode { get; set; }
    public string TargetLangCode { get; set; } = string.Empty;
    public string TargetScriptCode { get; set; } = string.Empty;
    public string Input { get; set; } = string.Empty;
    public string? ResultText { get; set; }
}