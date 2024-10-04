using FluentValidation;
using Holonet.Databank.Core.Dtos;
namespace Holonet.Databank.API.Validation;

public class CreateAuthorRequestDtoValidator : AbstractValidator<CreateAuthorDto>
{
	public CreateAuthorRequestDtoValidator()
	{
		RuleFor(x => x.AzureId)
			.NotEmpty();

		RuleFor(x => x.DisplayName)
			.NotEmpty()
			.Length(2, 255);

		RuleFor(x => x.Email)
			.EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
			.Length(0, 255);
	}
}
