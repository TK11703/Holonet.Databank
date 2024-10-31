using FluentValidation;
using Holonet.Databank.Core.Dtos;
namespace Holonet.Databank.API.Validation;

public class UpdateHistoricalEventDtoRequestValidator : AbstractValidator<UpdateHistoricalEventDto>
{
	public UpdateHistoricalEventDtoRequestValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty().WithMessage("Id is required.");

		RuleFor(x => x.Name)
			.NotEmpty().WithMessage("Name is required.")
			.Length(2, 150).WithMessage("Name must be no more than 150 characters in length.");

		RuleFor(x => x.Shard)
			.Length(0, 500).WithMessage("Shard must be no more than 500 characters in length.");

		RuleFor(x => x.DatePeriod)
			.Length(0, 200).WithMessage("Date period must be no more than 200 characters in length.");

		RuleForEach(x => x.Aliases)
			.Must(item => item.Length <= 150)
			.WithMessage("Each alias must be no more than 150 characters in length.");

		RuleFor(x => x.AzureId)
			.NotEmpty().WithMessage("AzureId is required.");
	}
}
