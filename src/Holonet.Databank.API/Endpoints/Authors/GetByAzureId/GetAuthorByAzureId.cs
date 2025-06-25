using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Web.Resource;

namespace Holonet.Databank.API.Endpoints.Authors.GetByAzureId;

public class GetAuthorByAzureId : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet($"/Authors/{{id}}", HandleAsync)
			.WithTags(Tags.Authors);
	}

	protected virtual async Task<Results<Ok<AuthorDto>, ProblemHttpResult, NotFound>> HandleAsync(Guid id, IAuthorService authorService)
	{
		try
		{
			var result = await authorService.GetAuthorByAzureId(id);
			if (result == null)
				return TypedResults.NotFound();
			return TypedResults.Ok(result.ToDto());
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}