using FluentValidation;
using Holonet.Databank.Core.Entities;

namespace Holonet.Databank.API.Validation;

public class TextSummaryRequestValidator : AbstractValidator<TextSummaryRequest>
{
    public TextSummaryRequestValidator()
    {
        RuleFor(x => x.Input)
            .NotEmpty();

        RuleFor(x => x.TargetLangCode)
            .NotEmpty()
            .WithMessage("The target language code must be supplied.");

        RuleFor(x => x)
            .Must(x => x.Summary || x.AbstractiveSummary)
            .WithMessage("At least one of the summary properties must be true.");

    }
}
