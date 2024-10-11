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
			.NotEmpty().WithMessage("Name is required.")
			.Length(2, 150).WithMessage("Name must be no more than 150 characters in length.");
	}
}
