using FluentValidation.Results;

namespace Holonet.Databank.API.Filters;

public class ValidationFailureResponse
{
	public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();
}

public static class ValidationFailureMapper
{
	public static ValidationFailureResponse ToResponse(this IEnumerable<ValidationFailure> failure)
	{
		return new ValidationFailureResponse
		{
			Errors = failure.Select(x => x.ErrorMessage),
		};
	}
}
