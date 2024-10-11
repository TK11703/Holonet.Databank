using Holonet.Databank.Application.Services;
using Holonet.Databank.API.Filters;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Holonet.Databank.Core.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web.Resource;

namespace Holonet.Databank.API.Endpoints.HistoricalEvents.Insert;

public class InsertNewHistoricalEvent : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost($"/HistoricalEvents", HandleAsync)
			.AddEndpointFilter<ValidatorFilter<CreateHistoricalEventDto>>()
			.WithTags(Tags.HistoricalEvents);
	}

	[Authorize]
	[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
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
				Aliases = itemModel.Aliases.Select(alias => new Alias { Name = alias, UpdatedBy = author }),
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
