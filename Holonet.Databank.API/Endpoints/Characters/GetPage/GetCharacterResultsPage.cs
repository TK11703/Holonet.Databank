using Holonet.Databank.Application.Services;
using Holonet.Databank.API.Filters;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Holonet.Databank.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Holonet.Databank.Core.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web.Resource;
namespace Holonet.Databank.API.Endpoints.Characters.GetPage;

public class GetCharacterResultsPage : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet($"/Characters/PagedRequest", HandleAsync)
			.AddEndpointFilter<ValidatorFilter<PageRequestDto>>()
			.WithTags(Tags.Characters);
	}
	protected virtual async Task<Results<Ok<PageResultDto<CharacterDto>>, ProblemHttpResult>> HandleAsync([FromBody] PageRequestDto pageRequest, ICharacterService characterService)
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
			PageResult<Character> pageResponse = await characterService.GetPagedAsync(modelRequest);
			return TypedResults.Ok(pageResponse.ToDto());
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
