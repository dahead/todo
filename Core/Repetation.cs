using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Spectre.Console;

public static class Repetation
{
    public enum RepeationType
    {
        EveryMinute,
        EveryHour,
        EveryDay,
        EveryWeek,
        EveryMonth,
        EveryYear
    }

    public static string[] GetRepetationTypeNames()
    {
      return Enum.GetNames(typeof(RepeationType));
    }

    public static IEnumerable<DateTime> FindInterval(DateTime from, RepeationType period, int count) 
    {
      while (true) 
      {
        switch (period) 
        {
          case RepeationType.EveryMinute:
            from = from.AddMinutes(count);
            break;
          case RepeationType.EveryHour:
            from = from.AddHours(count);
            break;
          case RepeationType.EveryDay:
            from = from.AddDays(count);
            break;
          case RepeationType.EveryWeek:
            from = from.AddDays(7 * count);
            break;
          case RepeationType.EveryMonth:
            from = from.AddMonths(count);
            break;
          case RepeationType.EveryYear:
            from = from.AddYears(count);
            break;
        } 
        yield return from;
      }
    }

    public class RepetationItem
    {
        public RepeationType RepetationType { get; set; }
        public DateTime RepeatAt { get; set; }        
    }    

    public class RepetationList : List<RepetationItem>
    {
        public void AddIntervals(DateTime from, RepeationType rt, int modifier, int amount)
        {
            List<DateTime> intervals = FindInterval(from, rt, modifier).Take(amount).ToList();
            foreach (var intervaldueat in intervals)
            {
                RepetationItem ri = new RepetationItem() 
                { 
                  RepetationType = rt, 
                  RepeatAt = intervaldueat 
                };
                this.Add(ri);
            }
        }
    }

}
