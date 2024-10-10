using FluentValidation;
using Holonet.Databank.Core.Dtos;
namespace Holonet.Databank.API.Validation;

public class CreateCharacterDtoRequestValidator : AbstractValidator<CreateCharacterDto>
{
	public CreateCharacterDtoRequestValidator()
	{
		RuleFor(x => x.GivenName)
			.NotEmpty()
			.Length(2, 150);

		RuleFor(x => x.FamilyName)
			.Length(0, 150);

		RuleFor(x => x.BirthDate)
			.Length(0, 200);

		RuleFor(x => x.Shard)
			.Length(0, 500);
	}
}
