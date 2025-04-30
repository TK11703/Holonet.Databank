using FluentValidation;
using Holonet.Databank.Core.Dtos;
namespace Holonet.Databank.API.Validation;

public class UpdateDataRecordDtoRequestValidator : AbstractValidator<UpdateRecordDto>
{
	public UpdateDataRecordDtoRequestValidator()
	{
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.Data)
			.NotEmpty().WithMessage("Data is required.");

        RuleFor(x => x.Shard)
            .Length(0, 500).WithMessage("Shard must be no more than 500 characters in length.");

        RuleFor(x => x.AzureId)
			.NotEmpty().WithMessage("AzureId is required.");
	}
}
