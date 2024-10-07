using FluentValidation;
using Holonet.Databank.Core.Dtos;
namespace Holonet.Databank.API.Validation;

public class GetCharacterDtoRequestValidator : AbstractValidator<GetCharacterDto>
{
	public GetCharacterDtoRequestValidator()
	{
		RuleFor(x => x.Id)
			.GreaterThanOrEqualTo(0);

		RuleFor(x => x.FirstName)
			.NotEmpty()
			.Length(2, 150);

		RuleFor(x => x.LastName)
			.NotEmpty()
			.Length(2, 150);
	}
}
