using System;

namespace NuffBot
{
  public static class Helpers
  {
    public static string FormatTimeSpan(TimeSpan span)
    {
      string formatted = "";

      if (span.Days / 365 > 0)
      {
        formatted += $"{span.Days / 365} years, ";
      }

      if (span.Days > 0)
      {
        formatted += $"{span.Days % 365} days, ";
      }

      if (span.Hours > 0)
      {
        formatted += $"{span.Hours} hours, ";
      }

      if (span.Minutes > 0)
      {
        formatted += $"{span.Minutes} minutes and ";
      }

      if (span.Seconds > 0)
      {
        formatted += $"{span.Seconds} seconds";
      }

      return formatted;
    }
  }
}