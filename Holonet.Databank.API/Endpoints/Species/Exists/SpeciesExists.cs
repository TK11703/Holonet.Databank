using Holonet.Databank.API.Filters;
using Holonet.Databank.Application.Services;
using Holonet.Databank.Core.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Web.Resource;

namespace Holonet.Databank.API.Endpoints.Species.Exists;

public class SpeciesExists : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost($"/Species/Exists", HandleAsync)
			.AddEndpointFilter<ValidatorFilter<GetSpeciesDto>>()
			.WithTags(Tags.Species);
	}

	[Authorize]
	[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
	protected virtual async Task<Results<Ok<bool>, ProblemHttpResult>> HandleAsync(GetSpeciesDto itemModel, ISpeciesService speciesService)
	{
		try
		{
			var exists = await speciesService.SpeciesExists(itemModel.Id, itemModel.Name);
			return TypedResults.Ok(exists);
		}
		catch (Exception ex)
		{
			return TypedResults.Problem(ex.Message);
		}
	}
}
