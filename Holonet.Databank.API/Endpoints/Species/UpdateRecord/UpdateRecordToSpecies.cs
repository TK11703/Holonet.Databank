using Holonet.Databank.API.Filters;
using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Holonet.Databank.API.Endpoints.Species.UpdateRecord;

public class UpdateRecordToSpecies : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost($"/Species/{{id}}/UpdateRecord/{{recordId}}", Handle)
			.AddEndpointFilter<ValidatorFilter<UpdateRecordDto>>()
			.WithTags(Tags.Species);
	}
	protected virtual async Task<Results<Ok<bool>, ProblemHttpResult>> Handle(int id, int recordId, UpdateRecordDto itemModel, IDataRecordService dataRecordService, IAuthorService authorService)
	{
		try
		{
			var author = await authorService.GetAuthorByAzureId(itemModel.UpdatedAzureId);
			if (author == null)
			{
				return TypedResults.Problem("Author not found");
			}
			var record = new DataRecord
			{
                Id = recordId,
                Data = itemModel.Data,
                Shard = itemModel.Shard,
                CharacterId = itemModel.CharacterId,
				HistoricalEventId = itemModel.HistoricalEventId,
				PlanetId = itemModel.PlanetId,
				SpeciesId = itemModel.SpeciesId,
				UpdatedBy = author
			};
			if(!record.SpeciesId.HasValue || !record.SpeciesId.Equals(id))
			{
				return TypedResults.Problem("Data record assignment did not match the item it was intended. Please resubmit with the correct identifiers.");
			}
			
			var rowsUpdated = await dataRecordService.UpdateDataRecord(record);
			return TypedResults.Ok(rowsUpdated);
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
