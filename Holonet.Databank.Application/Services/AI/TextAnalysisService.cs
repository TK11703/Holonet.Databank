using Azure;
using Azure.AI.TextAnalytics;
using Holonet.Databank.Core.Entities;
using System.Text;

namespace Holonet.Databank.Application.Services.AI;
public class TextAnalysisService
{
    private readonly TextAnalyticsClient _analysisClient;

    public TextAnalysisService(string serviceKey, string endPointUrl)
    {
        _analysisClient = new TextAnalyticsClient(new Uri(endPointUrl), new AzureKeyCredential(serviceKey));
    }

    public LanguageDetectionResult DetermineLanguage(string input)
    {
        DetectedLanguage detectedLanguage = _analysisClient.DetectLanguage(input);
        return new LanguageDetectionResult()
        {
            Input = input,
            Output = detectedLanguage.Name,
            Confidence = detectedLanguage.ConfidenceScore
        };
    }

    public async Task<string> GetSummaryAbstract(string input)
    {
        return await GetSummaryAbstract([input]);
    }

    public async Task<string> GetSummaryAbstract(IEnumerable<string> inputs)
    {
        AbstractiveSummarizeOperation operation = await _analysisClient.AbstractiveSummarizeAsync(WaitUntil.Completed, inputs);
        StringBuilder resultbuilder = new StringBuilder();
        foreach (AbstractiveSummarizeResultCollection documentsPerPage in operation.GetValues())
        {
            foreach (AbstractiveSummarizeResult documentResult in documentsPerPage)
            {
                if (documentResult.HasError)
                {
                    resultbuilder.AppendLine($"  Error!");
                    resultbuilder.AppendLine($"  Document error code: {documentResult.Error.ErrorCode}");
                    resultbuilder.AppendLine($"  Message: {documentResult.Error.Message}");
                    continue;
                }
                foreach (AbstractiveSummary summary in documentResult.Summaries)
                {
                    resultbuilder.Append(summary.Text.Replace("\n", " "));
                }
            }
        }
        return resultbuilder.ToString();
    }

    public async Task<IEnumerable<string>> GetKeyPhrases(string input)
    {
        try
        {
            KeyPhraseCollection keyPhraseCollection = await _analysisClient.ExtractKeyPhrasesAsync(input);
            return keyPhraseCollection.Select(x => x);
        }
        catch (RequestFailedException ex)
        {
            Exception resultException = new Exception($"Document Error Code: {ex.ErrorCode}{Environment.NewLine}Message: {ex.Message}", ex);
            throw resultException;
        }
    }

    public async Task<IEnumerable<string>> GetKeyPhrases(IEnumerable<string> inputs)
    {
        ExtractKeyPhrasesResultCollection keyPhrasesInDocuments = await _analysisClient.ExtractKeyPhrasesBatchAsync(inputs);
        List<string> keyPhraseResults = new List<string>();
        foreach (ExtractKeyPhrasesResult documentResult in keyPhrasesInDocuments)
        {
            if (documentResult.HasError)
            {
                Exception resultException = new Exception($"Document Error Code: {documentResult.Error.ErrorCode}{Environment.NewLine}Message: {documentResult.Error.Message}");
                throw resultException;
            }
            keyPhraseResults.AddRange(documentResult.KeyPhrases.Select(x => x));
        }
        return keyPhraseResults;
    }

    public async Task<TextAnalysisResult> GetSentiment(string input)
    {
        DocumentSentiment sentimentAnalysis = await _analysisClient.AnalyzeSentimentAsync(input);
        TextAnalysisResult result = new TextAnalysisResult()
        {
            Sentiment = sentimentAnalysis.Sentiment.ToString(),
            PositiveConfidenceScore = sentimentAnalysis.ConfidenceScores.Positive,
            NeutralConfidenceScore = sentimentAnalysis.ConfidenceScores.Neutral,
            NegativeConfidenceScore = sentimentAnalysis.ConfidenceScores.Negative,
        };

        foreach (SentenceSentiment sentimentInSentence in sentimentAnalysis.Sentences)
        {
            result.SentenceAnalysis.Add(new TextAnalysisResultBase()
            {
                Input = sentimentInSentence.Text,
                PositiveConfidenceScore = sentimentInSentence.ConfidenceScores.Positive,
                NeutralConfidenceScore = sentimentInSentence.ConfidenceScores.Neutral,
                NegativeConfidenceScore = sentimentInSentence.ConfidenceScores.Negative,
            });
        }
        return result;
    }

    public async Task<IEnumerable<TextAnalysisResult>> GetSentiment(IEnumerable<string> inputs)
    {
        AnalyzeSentimentResultCollection sentimentAnalysisResults = await _analysisClient.AnalyzeSentimentBatchAsync(inputs);
        List<TextAnalysisResult> results = new List<TextAnalysisResult>();
        foreach (AnalyzeSentimentResult sentimentResult in sentimentAnalysisResults)
        {
            if (sentimentResult.HasError)
            {
                Exception resultException = new Exception($"Document Error Code: {sentimentResult.Error.ErrorCode}{Environment.NewLine}Message: {sentimentResult.Error.Message}");
                throw resultException;
            }
            var result = new TextAnalysisResult()
            {
                Sentiment = sentimentResult.DocumentSentiment.Sentiment.ToString(),
                PositiveConfidenceScore = sentimentResult.DocumentSentiment.ConfidenceScores.Positive,
                NeutralConfidenceScore = sentimentResult.DocumentSentiment.ConfidenceScores.Neutral,
                NegativeConfidenceScore = sentimentResult.DocumentSentiment.ConfidenceScores.Negative
            };
            foreach (SentenceSentiment sentimentInSentence in sentimentResult.DocumentSentiment.Sentences)
            {
                result.SentenceAnalysis.Add(new TextAnalysisResultBase()
                {
                    Input = sentimentInSentence.Text,
                    PositiveConfidenceScore = sentimentInSentence.ConfidenceScores.Positive,
                    NeutralConfidenceScore = sentimentInSentence.ConfidenceScores.Neutral,
                    NegativeConfidenceScore = sentimentInSentence.ConfidenceScores.Negative,
                });
            }
            results.Add(result);
        }
        return results;
    }
}
