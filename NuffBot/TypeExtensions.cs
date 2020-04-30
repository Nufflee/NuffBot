using System;

namespace NuffBot
{
  public static class TypeExtensions
  {
    public static bool IsAssignableFromGeneric(this Type type, Type baseType)
    {
      while (!baseType.IsAssignableFrom(type))
      {
        if (type == null || type == typeof(object))
        {
          return false;
        }

        if (type.IsGenericType && !type.IsGenericTypeDefinition)
        {
          type = type.GetGenericTypeDefinition();
        }
        else
        {
          type = type.BaseType;
        }
      }

      return true;
    }
  }
}