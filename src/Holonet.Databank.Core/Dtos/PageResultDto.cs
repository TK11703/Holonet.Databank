using System.ComponentModel.DataAnnotations;

namespace Holonet.Databank.Core.Dtos;

public record PageResultDto<T>(
	int Start,
	int PageSize,
	int ItemCount,
	int TotalPages,
	int CurrentPage,
	bool IsFirstPage,
	bool IsLastPage,
	IEnumerable<T> Collection
);
