﻿using FluentValidation;
using Holonet.Databank.Core.Dtos;
using Holonet.Databank.Core.Models;

namespace Holonet.Databank.API.Validation;

public class PageRequestDtoValidator : AbstractValidator<PageRequestDto>
{
	public PageRequestDtoValidator()
	{
		RuleFor(x => x.Start)
			.GreaterThanOrEqualTo(0);

		RuleFor(x => x.PageSize)
			.NotEmpty().WithMessage("Page size is required.")
			.GreaterThanOrEqualTo(10)
			.LessThanOrEqualTo(100);

		RuleFor(x => x.BeginDate)
			.LessThanOrEqualTo(x => x.EndDate);

		RuleFor(x => x.EndDate)
			.GreaterThanOrEqualTo(x => x.BeginDate);

	}
}
