using Holonet.Databank.Application.Services;
using Holonet.Databank.API.Filters;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Holonet.Databank.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Holonet.Databank.Core.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web.Resource;
namespace Holonet.Databank.API.Endpoints.Species.GetPage;

public class GetSpeciesResultsPage : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet($"/Species/PagedRequest", HandleAsync)
			.AddEndpointFilter<ValidatorFilter<PageRequestDto>>()
			.WithTags(Tags.Species);
	}

	[Authorize]
	[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
	protected virtual async Task<Results<Ok<PageResultDto<SpeciesDto>>, ProblemHttpResult>> HandleAsync([FromBody] PageRequestDto pageRequest, ISpeciesService speciesService)
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

			PageResult<Core.Entities.Species> pageResponse = await speciesService.GetPagedAsync(modelRequest);
			return TypedResults.Ok(pageResponse.ToDto());
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
