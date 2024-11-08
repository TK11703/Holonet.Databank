using FluentValidation;
using Holonet.Databank.Core.Dtos;
namespace Holonet.Databank.API.Validation;

public class ChatRequestDtoRequestValidator : AbstractValidator<ChatRequestDto>
{
	public ChatRequestDtoRequestValidator()
	{
		RuleFor(x => x.Prompt)
			.NotEmpty().WithMessage("Prompt is required.");

		RuleFor(x => x.AzureId)
			.NotEmpty().WithMessage("AzureId is required.");
	}
}
