﻿using FluentValidation;

namespace Holonet.Databank.API.Filters;

public class ValidatorFilter<T> : IEndpointFilter where T : class
{
	private readonly IValidator<T> _validator;

	public ValidatorFilter(IValidator<T> validator)
	{
		_validator = validator;
	}

	public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		var validatable = context.Arguments.SingleOrDefault(x => x?.GetType() == typeof(T)) as T;
		if (validatable is null)
		{
			return Results.BadRequest();
		}
		var validationResult = await _validator.ValidateAsync(validatable);
		if (!validationResult.IsValid)
		{
			return Results.BadRequest(validationResult.Errors.ToResponse());
		}
		return await next(context);
	}
}