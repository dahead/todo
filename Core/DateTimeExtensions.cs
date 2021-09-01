using System;
using System.Collections.Generic;
using System.Linq;

public static class DateTimeExtensions
{

	public static string ToReadableStringSimple(this TimeSpan span)
	{
		return new DateTime(span.Ticks).ToString("MMM dd yyyy hh:mm:ss");
	}

	public static string ToReadableString(this TimeSpan span)
	{
		return string.Join(", ", span.GetReadableStringElements()
		   .Where(str => !string.IsNullOrWhiteSpace(str)));
	}

	private static IEnumerable<string> GetReadableStringElements(this TimeSpan span)
	{
		yield return GetDaysString((int)Math.Floor(span.TotalDays));
		yield return GetHoursString(span.Hours);
		yield return GetMinutesString(span.Minutes);
		yield return GetSecondsString(span.Seconds);
	}

	private static string GetDaysString(int days)
	{
		if (days == 0)
			return string.Empty;

		if (days == 1)
			return "1 day";

		return string.Format("{0:0} days", days);
	}

	private static string GetHoursString(int hours)
	{
		if (hours == 0)
			return string.Empty;

		if (hours == 1)
			return "1 hour";

		return string.Format("{0:0} hours", hours);
	}

	private static string GetMinutesString(int minutes)
	{
		if (minutes == 0)
			return string.Empty;
		if (minutes == 1)
			return "1 minute";
		return string.Format("{0:0} minutes", minutes);
	}

	private static string GetSecondsString(int seconds)
	{
		if (seconds == 0)
			return string.Empty;
		if (seconds == 1)
			return "1 second";
		return string.Format("{0:0} S", seconds);
	}

	public static string TimeAgo(DateTime dt)
	{
		TimeSpan span = DateTime.Now - dt;
		if (span.Days > 365)
		{
			int years = (span.Days / 365);
			if (span.Days % 365 != 0)
				years += 1;
			return String.Format("about {0} {1} ago", years, years == 1 ? "year" : "years");
		}

		if (span.Days > 30)
		{
			int months = (span.Days / 30);
			if (span.Days % 31 != 0)
				months += 1;
			return String.Format("about {0} {1} ago", months, months == 1 ? "month" : "months");
		}

		if (span.Days > 0)
			return String.Format("about {0} {1} ago", span.Days, span.Days == 1 ? "day" : "days");

		if (span.Hours > 0)
			return String.Format("about {0} {1} ago", span.Hours, span.Hours == 1 ? "hour" : "hours");
		if (span.Minutes > 0)
			return String.Format("about {0} {1} ago", span.Minutes, span.Minutes == 1 ? "minute" : "minutes");
		if (span.Seconds > 5)
			return String.Format("about {0} seconds ago", span.Seconds);

		if (span.Seconds <= 5)
			return "just now";

		return string.Empty;
	}

	public static string GetAgeOf(DateTimeOffset createdAt)
	{
		var tsSinceCreation = DateTimeOffset.UtcNow - createdAt;
		return tsSinceCreation switch
		{

			TimeSpan Age when Age < TimeSpan.FromHours(24) => $"{Math.Ceiling(tsSinceCreation.TotalDays)} days",
			//TimeSpan Age when Age < TimeSpan.FromHours(0) => $"{Math.Ceiling(tsSinceCreation.TotalDays)} days",

			TimeSpan Age when Age < TimeSpan.FromHours(1) => $"{Math.Ceiling(tsSinceCreation.TotalMinutes)} minutes",

			TimeSpan Age when Age >= TimeSpan.FromHours(1) && Age < TimeSpan.FromHours(2) => $"{Math.Floor(tsSinceCreation.TotalHours)} hour",
			TimeSpan Age when Age >= TimeSpan.FromHours(2) && Age < TimeSpan.FromHours(24) => $"{Math.Floor(tsSinceCreation.TotalHours)} hours",
			TimeSpan Age when Age >= TimeSpan.FromHours(24) && Age < TimeSpan.FromHours(48) => $"{Math.Floor(tsSinceCreation.TotalDays)} day",
			TimeSpan Age when Age >= TimeSpan.FromHours(48) => $"{Math.Floor(tsSinceCreation.TotalDays)} days",
			_ => string.Empty,
		};
	}

}