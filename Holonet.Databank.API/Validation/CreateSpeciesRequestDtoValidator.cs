using FluentValidation;
using Holonet.Databank.Core.Dtos;
namespace Holonet.Databank.API.Validation;

public class CreateSpeciesRequestDtoValidator : AbstractValidator<CreateSpeciesDto>
{
	public CreateSpeciesRequestDtoValidator()
	{
		RuleFor(x => x.Name)
			.NotEmpty()
			.Length(2, 150);

		RuleFor(x => x.Shard)
			.Length(0, 500);
	}
}
