using Holonet.Databank.Core.Dtos;

namespace Holonet.Databank.Core.Models;
public class PageRequest
{
	public int Start { get; set; } = 0;
	public int PageSize { get; set; }
	public DateTime? BeginDate { get; set; }
	public DateTime? EndDate { get; set; }
	public string? Filter { get; set; } = string.Empty;
	public string? SortBy { get; set; }
	public string? SortDirection { get; set; }
	public string? DateRange
	{
		get
		{
			if (BeginDate.HasValue && EndDate.HasValue)
			{
				return $"{BeginDate.Value.ToString("MM/dd/yyyy")} - {EndDate.Value.ToString("MM/dd/yyyy")}";
			}
			return string.Empty;
		}
	}

	public PageRequest()
	{

	}

	public bool ContainsFilter()
	{
		if (!string.IsNullOrEmpty(Filter) || BeginDate.HasValue && EndDate.HasValue)
			return true;
		return false;
	}

	public void SetDefaults(int page, int pageSize, string sortBy, string sortDirection)
	{
		SetPage(page);
		PageSize = pageSize;
		SortBy = sortBy;
		SortDirection = sortDirection;
	}

	public void SetPage(int page)
	{
		Start = PageSize * page - PageSize;
	}
}