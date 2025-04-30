using Holonet.Databank.API.Filters;
using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Holonet.Databank.API.Endpoints.Planets.AddRecord;

public class AddRecordToPlanet : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost($"/Planets/{{id}}/AddRecord", Handle)
			.AddEndpointFilter<ValidatorFilter<CreateRecordDto>>()
			.WithTags(Tags.Planets);
	}
	protected virtual async Task<Results<Ok<bool>, ProblemHttpResult>> Handle(int id, CreateRecordDto itemModel, IDataRecordService dataRecordService, IAuthorService authorService)
	{
		try
		{
			var author = await authorService.GetAuthorByAzureId(itemModel.AzureId);
			if (author == null)
			{
				return TypedResults.Problem("Author not found");
			}
			var record = new DataRecord
			{
				Data = itemModel.Data,
                Shard = itemModel.Shard,
                CharacterId = itemModel.CharacterId,
				HistoricalEventId = itemModel.HistoricalEventId,
				PlanetId = itemModel.PlanetId,
				SpeciesId = itemModel.SpeciesId,
				UpdatedBy = author
			};
			if(!record.PlanetId.HasValue || !record.PlanetId.Equals(id))
			{
				return TypedResults.Problem("Data record assignment did not match the item it was intended. Please resubmit with the correct identifiers.");
			}
			
			var rowsUpdated = await dataRecordService.CreateDataRecord(record);
			return TypedResults.Ok(rowsUpdated);
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
