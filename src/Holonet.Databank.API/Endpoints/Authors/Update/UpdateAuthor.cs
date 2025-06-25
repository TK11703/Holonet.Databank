using Holonet.Databank.API.Filters;
using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Holonet.Databank.API.Endpoints.Authors.Update;

public class UpdateAuthor : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPut($"/Authors/{{id}}", Handle)
			.AddEndpointFilter<ValidatorFilter<UpdateAuthorDto>>()
			.WithTags(Tags.Authors);
	}
	protected virtual Results<Ok<bool>, ProblemHttpResult> Handle(int id, UpdateAuthorDto itemModel, IAuthorService authorService)
	{
		try
		{
			var author = new Author
			{
				Id = itemModel.Id,
				AzureId = itemModel.AzureId,
				DisplayName = itemModel.DisplayName,
				Email = itemModel.Email
			};
			var rowsUpdated = authorService.UpdateAuthor(author);
			return TypedResults.Ok(rowsUpdated);
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
