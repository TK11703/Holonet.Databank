using FluentValidation;
using Holonet.Databank.Core.Dtos;
namespace Holonet.Databank.API.Validation;

public class CreateDataRecordDtoRequestValidator : AbstractValidator<CreateRecordDto>
{
	public CreateDataRecordDtoRequestValidator()
	{
		RuleFor(x => x.Data)
			.NotEmpty().WithMessage("Data is required.");

		RuleFor(x => x.AzureId)
			.NotEmpty().WithMessage("AzureId is required.");
	}
}
