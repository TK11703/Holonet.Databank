﻿using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Holonet.Databank.Application.AICapabilities.Plugins;
public class HolonetSearchPlugin(IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, SearchIndexClient indexClient, string indexName)
{
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator = embeddingGenerator;
    private readonly SearchIndexClient _indexClient = indexClient;
    private readonly string _indexName = indexName;

    [KernelFunction("holonet_search")]
    [Description("Queries the Holonet mainframe for details on Star Wars characters, planets, species, historical events and includes the source of the data.")]
    [return: Description("A formatted response that contains details of the first response from the Holonet mainframe. The search result contains the details and the source of the data.")]
    public async Task<string> SearchAsync(string query)
    {
        // Convert string query to vector
        Embedding<float> embedding = await _embeddingGenerator.GenerateAsync(query);

        // Get client for search operations
        SearchClient searchClient = _indexClient.GetSearchClient(_indexName);

        // Configure request parameters
        VectorizedQuery vectorQuery = new(embedding.Vector);
        vectorQuery.Fields.Add("text_vector");

        SearchOptions searchOptions = new() { VectorSearch = new() { Queries = { vectorQuery } } };

        // Perform search request
        SearchResults<IndexSchema> results = await searchClient.SearchAsync<IndexSchema>(searchOptions);

        return await ProcessFirstResultAsync(results);
    }

    private static async Task<string> ProcessFirstResultAsync(SearchResults<IndexSchema> results)
    {
        string resultText = string.Empty;
        List<SearchResult<IndexSchema>> resultList = new();
        await foreach (SearchResult<IndexSchema> result in results.GetResultsAsync())
        {
            resultList.Add(result);
        }
        if (resultList.Count > 0)
        {
            SearchResult<IndexSchema> result = resultList[0];
            resultText = $"Details: {result.Document.Chunk}\nSource: Holonet Mainframe";
        }

        return resultText;
    }

    private sealed class IndexSchema
    {
        [JsonPropertyName("chunk")]
        public string Chunk { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
    }
}
