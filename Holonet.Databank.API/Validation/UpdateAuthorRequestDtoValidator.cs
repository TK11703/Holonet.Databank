using FluentValidation;
using Holonet.Databank.Core.Dtos;
namespace Holonet.Databank.API.Validation;

public class UpdateAuthorRequestDtoValidator : AbstractValidator<UpdateAuthorDto>
{
	public UpdateAuthorRequestDtoValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty().WithMessage("Id is required.");

		RuleFor(x => x.AzureId)
			.NotEmpty().WithMessage("Azure Id is required.");

		RuleFor(x => x.DisplayName)
			.NotEmpty().WithMessage("Display name is required.")
			.Length(2, 255).WithMessage("Display name must be no more than 150 characters in length.");

		RuleFor(x => x.Email)
			.EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
			.Length(0, 255).WithMessage("Email must be no more than 255 characters in length.");
	}
}
