
namespace Holonet.Databank.Core.Entities;
public class LanguageDetectionResult
{
    public string? Input { get; set; }
    public string? Output { get; set; }
    public double Confidence { get; set; }
}