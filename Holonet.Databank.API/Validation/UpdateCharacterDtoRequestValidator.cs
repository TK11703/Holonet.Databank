using FluentValidation;
using Holonet.Databank.Core.Dtos;
namespace Holonet.Databank.API.Validation;

public class UpdateCharacterDtoRequestValidator : AbstractValidator<UpdateCharacterDto>
{
	public UpdateCharacterDtoRequestValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty();

		RuleFor(x => x.FirstName)
			.NotEmpty()
			.Length(2, 150);

		RuleFor(x => x.LastName)
			.NotEmpty()
			.Length(2, 150);

		RuleFor(x => x.BirthDate)
			.Length(0, 200);

		RuleFor(x => x.Shard)
			.Length(0, 500);
	}
}
