﻿using Holonet.Databank.Application.Services;
using Holonet.Databank.API.Filters;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Holonet.Databank.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Holonet.Databank.Core.Dtos;

namespace Holonet.Databank.API.Endpoints.Planets.GetPage;

public class GetPlanetsResultsPage : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost($"/Planets/PagedRequest", HandleAsync)
			.AddEndpointFilter<ValidatorFilter<PageRequestDto>>()
			.WithTags(Tags.Planets);
	}
	protected virtual async Task<Results<Ok<PageResultDto<PlanetDto>>, ProblemHttpResult>> HandleAsync([FromBody] PageRequestDto pageRequest, IPlanetService planetService)
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

			PageResult<Planet> pageResponse = await planetService.GetPagedAsync(modelRequest);
			return TypedResults.Ok(pageResponse.ToDto());
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
