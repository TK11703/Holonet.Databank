using Holonet.Databank.Application.Services;
using Holonet.Databank.API.Filters;
using Holonet.Databank.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Holonet.Databank.Core.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web.Resource;

namespace Holonet.Databank.API.Endpoints.Planets.Insert;

public class InsertNewPlanet : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost($"/Planets", HandleAsync)
			.AddEndpointFilter<ValidatorFilter<CreatePlanetDto>>()
			.WithTags(Tags.Planets);
	}

	[Authorize]
	[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
	protected virtual async Task<Results<Ok<int>, ProblemHttpResult>> HandleAsync(CreatePlanetDto itemModel, IPlanetService planetService, IAuthorService authorService, IUserService userService)
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
			var newPlanet = new Planet
			{
				Name = itemModel.Name,
				Description = itemModel.Description,
				Shard = itemModel.Shard,
				UpdatedBy = author
			};
			int newId = await planetService.CreatePlanet(newPlanet);
			return TypedResults.Ok(newId);
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
