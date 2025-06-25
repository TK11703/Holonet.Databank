using FluentValidation;
using Holonet.Databank.Core.Dtos;
namespace Holonet.Databank.API.Validation;

public class UpdateCharacterDtoRequestValidator : AbstractValidator<UpdateCharacterDto>
{
	public UpdateCharacterDtoRequestValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty().WithMessage("Id is required.");

		RuleFor(x => x.GivenName)
			.NotEmpty().WithMessage("Given name is required.")
			.Length(2, 150).WithMessage("Given name must be no more than 150 characters in length.");

		RuleFor(x => x.FamilyName)
			.Length(0, 150).WithMessage("Family name must be no more than 150 characters in length.");

		RuleFor(x => x.BirthDate)
			.Length(0, 200).WithMessage("Birth date must be no more than 200 characters in length.");

		RuleForEach(x => x.Aliases)
			.Must(item => item.Length <= 150)
			.WithMessage("Each alias must be no more than 150 characters in length.");

		RuleFor(x => x.AzureId)
			.NotEmpty().WithMessage("AzureId is required.");
	}
}
