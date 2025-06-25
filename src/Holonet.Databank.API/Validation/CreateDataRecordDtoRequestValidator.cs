using FluentValidation;
using Holonet.Databank.Core.Dtos;
namespace Holonet.Databank.API.Validation;

public class CreateDataRecordDtoRequestValidator : AbstractValidator<CreateRecordDto>
{
	public CreateDataRecordDtoRequestValidator()
	{
		RuleFor(x => x.Shard)
            .Length(0, 500).WithMessage("Shard must be no more than 500 characters in length.");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.Shard) || !string.IsNullOrEmpty(x.Data))
            .WithMessage("Either a shard or a data entry is needed.");

        RuleFor(x => x.CreatedAzureId)
			.NotEmpty().WithMessage("CreatedAzureId is required.");
	}
}
