﻿using Holonet.Databank.API.Filters;
using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
namespace Holonet.Databank.API.Endpoints.Species.Update;

public class UpdateSpecies : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPut($"/Species/{{id}}", Handle)
			.AddEndpointFilter<ValidatorFilter<UpdateSpeciesDto>>()
			.WithTags(Tags.Species);
	}
	protected virtual async Task<Results<Ok<bool>, ProblemHttpResult>> Handle(int id, UpdateSpeciesDto itemModel, ISpeciesService speciesService, IAuthorService authorService)
	{
		try
		{
			var author = await authorService.GetAuthorByAzureId(itemModel.AzureId);
			if (author == null)
			{
				return TypedResults.Problem("Author not found");
			}
			var species = new Core.Entities.Species
            {
				Id = itemModel.Id,
				Name = itemModel.Name,
				Aliases = itemModel.Aliases.Select(alias => new Alias { Name = alias, UpdatedBy = author }),
				UpdatedBy = author
			};
			var rowsUpdated = await speciesService.UpdateSpecies(species);
			return TypedResults.Ok(rowsUpdated);
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
