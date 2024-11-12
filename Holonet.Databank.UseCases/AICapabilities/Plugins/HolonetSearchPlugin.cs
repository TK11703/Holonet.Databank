using System.ComponentModel;
using System.Text.Json.Serialization;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;

namespace Holonet.Databank.Application.AICapabilities.Plugins;
public class HolonetSearchPlugin
{
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    private readonly ITextEmbeddingGenerationService _textEmbeddingGenerationService;
    private readonly SearchIndexClient _indexClient;
    private readonly string _indexName;

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    public HolonetSearchPlugin(ITextEmbeddingGenerationService textEmbeddingGenerationService, SearchIndexClient indexClient, IConfiguration configuration)
    {
        _textEmbeddingGenerationService = textEmbeddingGenerationService;
        _indexClient = indexClient;
        _indexName = configuration["AzureAiSearch:Index"] ?? throw new MissingFieldException("AzureAiSearch:Index");
    }

    [KernelFunction("holonet_search")]
    [Description("Search documents similar to the given query.")]
    public async Task<string> SearchAsync(string query)
    {
        // Convert string query to vector
        ReadOnlyMemory<float> embedding = await _textEmbeddingGenerationService.GenerateEmbeddingAsync(query);

        // Get client for search operations
        SearchClient searchClient = _indexClient.GetSearchClient(_indexName);

        // Configure request parameters
        VectorizedQuery vectorQuery = new(embedding);
        vectorQuery.Fields.Add("contentVector");

        SearchOptions searchOptions = new() { VectorSearch = new() { Queries = { vectorQuery } } };

        // Perform search request
        Response<SearchResults<IndexSchema>> response = await searchClient.SearchAsync<IndexSchema>(searchOptions);

        // Collect search results
        await foreach (SearchResult<IndexSchema> result in response.Value.GetResultsAsync())
        {
            return result.Document.Content; // Return text from first result
        }

        return string.Empty;
    }

    private sealed class IndexSchema
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
