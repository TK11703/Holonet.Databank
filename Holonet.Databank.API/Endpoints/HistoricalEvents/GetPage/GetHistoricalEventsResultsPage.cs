using Holonet.Databank.Application.Services;
using Holonet.Databank.API.Filters;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Holonet.Databank.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Holonet.Databank.Core.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web.Resource;
namespace Holonet.Databank.API.Endpoints.HistoricalEvents.GetPage;

public class GetHistoricalEventsResultsPage : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet($"/HistoricalEvents/PagedRequest", HandleAsync)
			.AddEndpointFilter<ValidatorFilter<PageRequestDto>>()
			.WithTags(Tags.HistoricalEvents);
	}
	protected virtual async Task<Results<Ok<PageResultDto<HistoricalEventDto>>, ProblemHttpResult>> HandleAsync([FromBody] PageRequestDto pageRequest, IHistoricalEventService historicalEventService)
	{
		try
		{
			PageRequest modelRequest = new()
			{
				Start = pageRequest.Start,
				PageSize = pageRequest.PageSize,
				BeginDate = pageRequest.BeginDate,
				EndDate = pageRequest.EndDate,
				Filter = pageRequest.Filter,
				SortBy = pageRequest.SortBy,
				SortDirection = pageRequest.SortDirection
			};
			PageResult<HistoricalEvent> pageResponse = await historicalEventService.GetPagedAsync(modelRequest);
			return TypedResults.Ok(pageResponse.ToDto());
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
