using FluentValidation;
using Holonet.Databank.Core.Dtos;
namespace Holonet.Databank.API.Validation;

public class CreatePlanetRequestDtoValidator : AbstractValidator<CreatePlanetDto>
{
	public CreatePlanetRequestDtoValidator()
	{
		RuleFor(x => x.Name)
			.NotEmpty()
			.Length(2, 150);

		RuleFor(x => x.Shard)
			.Length(0, 500);
	}
}
