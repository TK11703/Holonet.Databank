using Holonet.Databank.Application.Services;
using Holonet.Databank.API.Filters;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Holonet.Databank.Core.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web.Resource;

namespace Holonet.Databank.API.Endpoints.Species.Insert;

public class InsertNewSpecies : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost($"/Species", HandleAsync)
			.AddEndpointFilter<ValidatorFilter<CreateSpeciesDto>>()
			.WithTags(Tags.Species);
	}
	protected virtual async Task<Results<Ok<int>, ProblemHttpResult>> HandleAsync(CreateSpeciesDto itemModel, ISpeciesService speciesService, IAuthorService authorService)
	{
		try
		{
			var author = await authorService.GetAuthorByAzureId(itemModel.AzureId);
			if (author == null)
			{
				return TypedResults.Problem("Author not found");
			}
			var newSpecies = new Core.Entities.Species
            {
				Name = itemModel.Name,
				Aliases = itemModel.Aliases.Select(alias => new Alias { Name = alias, UpdatedBy = author }),
				UpdatedBy = author
			};
			int newId = await speciesService.CreateSpecies(newSpecies);
			return TypedResults.Ok(newId);
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
