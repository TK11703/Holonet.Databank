using Holonet.Databank.API.Filters;
using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Holonet.Databank.API.Endpoints.Species.AddRecord;

public class AddRecordToSpecies : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost($"/Species/{{id}}/AddRecord", Handle)
            .AddEndpointFilter<ValidatorFilter<CreateRecordDto>>()
            .WithTags(Tags.Species);
    }
    protected virtual async Task<Results<Ok<int>, ProblemHttpResult>> Handle(int id, CreateRecordDto itemModel, IDataRecordService dataRecordService, IAuthorService authorService)
    {
        try
        {
            var author = await authorService.GetAuthorByAzureId(itemModel.CreatedAzureId);
            if (author == null)
            {
                return TypedResults.Problem("Author not found");
            }
            var record = new DataRecord
            {
                Data = itemModel.Data,
                Shard = itemModel.Shard,
                IsNew = true,
                IsProcessing = false,
                IsProcessed = false,
                CharacterId = itemModel.CharacterId,
                HistoricalEventId = itemModel.HistoricalEventId,
                PlanetId = itemModel.PlanetId,
                SpeciesId = itemModel.SpeciesId,
                CreatedBy = author,
                UpdatedBy = author
            };
            if (!record.SpeciesId.HasValue || !record.SpeciesId.Equals(id))
            {
                return TypedResults.Problem("Data record assignment did not match the item it was intended. Please resubmit with the correct identifiers.");
            }

            int? newItemId = await dataRecordService.CreateDataRecord(record);
            if (newItemId.HasValue)
            {
                return TypedResults.Ok(newItemId.Value);
            }
            else
            {
                return TypedResults.Problem("Data record was not created, as an Id was not returned.");
            }
        }
        catch (Exception ex)
        {
            return TypedResults.Problem(ex.Message);
        }
    }
}
