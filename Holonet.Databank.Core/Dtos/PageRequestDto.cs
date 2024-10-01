using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;

public record PageRequestDto(
	int Start,
	int PageSize,
	DateTime? BeginDate,
	DateTime? EndDate,
	string? Filter,
	string? SortBy,
	string? SortDirection
);