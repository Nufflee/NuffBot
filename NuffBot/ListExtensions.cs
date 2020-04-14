using System.Collections.Generic;
using System;

namespace NuffBot
{
  public static class ListExtensions
  {
    public static T Random<T>(this List<T> list)
    {
      if (list.Count == 0)
      {
        return default;
      }
      
      return list[new Random().Next(list.Count)];
    }
  }
}