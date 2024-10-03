using Holonet.Databank.Application.Services;
using Holonet.Databank.API.Filters;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Holonet.Databank.Core.Dtos;

namespace Holonet.Databank.API.Endpoints.HistoricalEvents.Insert;

public class InsertNewHistoricalEvent : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost($"/HistoricalEvents", HandleAsync)
			.AddEndpointFilter<ValidatorFilter<CreateHistoricalEventDto>>()
			.WithTags(Tags.HistoricalEvents);
	}

	protected virtual async Task<Results<Ok<int>, ProblemHttpResult>> HandleAsync(CreateHistoricalEventDto itemModel, IHistoricalEventService historicalEventService, IAuthorService authorService, IUserService userService)
	{
		try
		{
			var azureId = userService.GetAzureId();
			if (azureId == null)
			{
				return TypedResults.Problem("User not found");
			}
			var author = await authorService.GetAuthorByAzureId(azureId.Value);
			if (author == null)
			{
				return TypedResults.Problem("Author not found");
			}
			var newHistoricalEvent = new HistoricalEvent
			{
				Name = itemModel.Name,
				Description = itemModel.Description,
				DatePeriod = itemModel.DatePeriod,
				CharacterIds = itemModel.CharacterIds,
				PlanetIds = itemModel.PlanetIds,
				Shard = itemModel.Shard,
				UpdatedBy = author
			};
			int newId = await historicalEventService.CreateHistoricalEvent(newHistoricalEvent);
			return TypedResults.Ok(newId);
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
