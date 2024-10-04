using FluentValidation;
using Holonet.Databank.Core.Dtos;
namespace Holonet.Databank.API.Validation;

public class UpdateAuthorRequestDtoValidator : AbstractValidator<UpdateAuthorDto>
{
	public UpdateAuthorRequestDtoValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty();

		RuleFor(x => x.AzureId)
			.NotEmpty();

		RuleFor(x => x.DisplayName)
			.NotEmpty()
			.Length(2, 255);

		RuleFor(x => x.Email)
			.EmailAddress()
			.Length(0, 255);
	}
}
