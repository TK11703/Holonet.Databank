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
			.NotEmpty()
			.Length(2, 150);

		RuleFor(x => x.FamilyName)
			.Length(0, 150);
	}
}
