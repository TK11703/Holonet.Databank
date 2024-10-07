using FluentValidation;
using Holonet.Databank.Core.Dtos;
namespace Holonet.Databank.API.Validation;

public class GetPlanetRequestDtoValidator : AbstractValidator<GetPlanetDto>
{
	public GetPlanetRequestDtoValidator()
	{
		RuleFor(x => x.Id)
			.GreaterThanOrEqualTo(0);

		RuleFor(x => x.Name)
			.NotEmpty()
			.Length(2, 150);
	}
}
