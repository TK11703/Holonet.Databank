using FluentValidation;
using Holonet.Databank.Core.Dtos;
namespace Holonet.Databank.API.Validation;

public class GetCharacterDtoRequestValidator : AbstractValidator<GetCharacterDto>
{
	public GetCharacterDtoRequestValidator()
	{
		RuleFor(x => x.Id)
			.GreaterThanOrEqualTo(0);

		RuleFor(x => x.GivenName)
			.NotEmpty().WithMessage("Given name is required.")
			.Length(2, 150).WithMessage("Given name must be no more than 150 characters in length.");

		RuleFor(x => x.FamilyName)
			.Length(0, 150).WithMessage("Family name must be no more than 150 characters in length.");
	}
}
