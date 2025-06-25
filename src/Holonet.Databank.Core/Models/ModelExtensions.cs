

using Holonet.Databank.Core.Dtos;

namespace Holonet.Databank.Core.Models;
public static class ModelExtensions
{
	public static PageRequestDto ToPageRequestDto(this PageRequest pageRequest)
	{
		return new PageRequestDto
		(
			Start: pageRequest.Start,
			PageSize: pageRequest.PageSize,
			BeginDate: pageRequest.BeginDate,
			EndDate: pageRequest.EndDate,
			Filter: pageRequest.Filter,
			SortBy: pageRequest.SortBy,
			SortDirection: pageRequest.SortDirection
		);
	}

	public static PageRequest ToPageRequest(this PageRequestDto pageRequestDto)
	{
		return new PageRequest()
		{
			Start = pageRequestDto.Start,
			PageSize = pageRequestDto.PageSize,
			BeginDate = pageRequestDto.BeginDate,
			EndDate = pageRequestDto.EndDate,
			Filter = pageRequestDto.Filter,
			SortBy = pageRequestDto.SortBy,
			SortDirection = pageRequestDto.SortDirection
		};
	}

	public static PageResult<T2> ToPageResult<T1, T2>(this PageResultDto<T1> pageResultDto)
	{
		return new PageResult<T2>
		{
			Start = pageResultDto.Start,
			PageSize = pageResultDto.PageSize,
			ItemCount = pageResultDto.ItemCount,
			Collection = []
		};
	}
}
