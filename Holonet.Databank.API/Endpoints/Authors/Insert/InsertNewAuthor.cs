using Holonet.Databank.API.Filters;
using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Holonet.Databank.API.Endpoints.Authors.Insert;

public class InsertNewAuthor : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost($"/Authors", HandleAsync)
			.AddEndpointFilter<ValidatorFilter<CreateAuthorDto>>()
			.WithTags(Tags.Authors);
	}
	protected virtual async Task<Results<Ok<int>, ProblemHttpResult>> HandleAsync(CreateAuthorDto itemModel, IAuthorService authorService)
	{
		try
		{
			var newAuthor = new Author
			{
				AzureId = itemModel.AzureId,
				DisplayName = itemModel.DisplayName,
				Email = itemModel.Email
			};
			int newId = await authorService.CreateAuthor(newAuthor);
			return TypedResults.Ok(newId);
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}