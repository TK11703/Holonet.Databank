﻿
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Holonet.Databank.Application.AICapabilities.Plugins;
public class UtcPlugin
{
	public UtcPlugin()
    {
    }

    private static readonly string DATETIMMEFORMAT = "MM-dd-yyyy HH:mm:ss";

	[KernelFunction("get_current_date_time_local")]
	[Description("Gets the current date and time in local timezone")]
	[return: Description("A string representing the current date and time of the local timezone")]
	public static string GetCurrentDateTime()
	{
		return DateTime.Now.ToString(DATETIMMEFORMAT);
	}

	[KernelFunction("get_current_date_time_utc")]
	[Description("Gets the current date and time in UTC")]
	[return: Description("A string representing the current UTC date and time")]
	public static string GetCurrentDateTimeUTC()
	{
		return DateTime.UtcNow.ToString(DATETIMMEFORMAT);
	}

	[KernelFunction("get_year")]
	[Description("Gets the year for a date parameter.")]
	[return: Description("An integer value representing the year of the date parameter")]
	public static int GetYear(DateTime? dateObj)
	{
		if (dateObj == null)
		{
			throw new ArgumentNullException(nameof(dateObj), "Date object is required.");
		}
		return dateObj.Value.Year;
	}

	[KernelFunction("get_month")]
	[Description("Gets the month for a date parameter.")]
	[return: Description("An integer value representing the month of the date parameter")]
	public static int GetMonth(DateTime? dateObj)
	{
		if (dateObj == null)
		{
			throw new ArgumentNullException(nameof(dateObj), "Date object is required.");
		}
		return dateObj.Value.Month;
	}

	[KernelFunction("get_day_of_week")]
	[Description("Gets the day of the week for a date parameter.")]
	[return: Description("An string value representing the day of the week from the date parameter")]
	public static string GetDayOfWeek(DateTime? dateObj)
	{
		if (dateObj == null)
		{
			throw new ArgumentNullException(nameof(dateObj), "Date object is required.");
		}
		return dateObj.Value.DayOfWeek.ToString();
	}
}
