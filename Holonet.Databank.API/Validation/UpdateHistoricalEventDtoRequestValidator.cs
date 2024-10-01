using FluentValidation;
using Holonet.Databank.Core.Dtos;
namespace Holonet.Databank.API.Validation;

public class UpdateHistoricalEventDtoRequestValidator : AbstractValidator<UpdateHistoricalEventDto>
{
	public UpdateHistoricalEventDtoRequestValidator()
	{
		RuleFor(x => x.Id)
			.NotEmpty();

		RuleFor(x => x.Name)
			.NotEmpty()
			.Length(2, 150);

		RuleFor(x => x.Shard)
			.Length(0, 500);

		RuleFor(x => x.DatePeriod)
			.Length(0, 200);
	}
}
