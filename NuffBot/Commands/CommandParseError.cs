using System;

namespace NuffBot.Commands
{
  public enum CommandParseErrorKind
  {
    ExpectedWordAtEnd,
    MissingOpeningBracket,
    MissingComma,
    MissingClosingBracket
  }

  public class CommandParseError : Exception
  {
    public CommandParseErrorKind Kind { get; }

    public CommandParseError(CommandParseErrorKind kind)
      : base(KindToString(kind))
    {
      Kind = kind;
    }

    private static string KindToString(CommandParseErrorKind kind)
    {
      switch (kind)
      {
        case CommandParseErrorKind.ExpectedWordAtEnd:
          return "Expected a word at the end";
        case CommandParseErrorKind.MissingOpeningBracket:
          return "Missing '[' at the start of an array";
        case CommandParseErrorKind.MissingComma:
          return "Missing ',' after an array element.";
        case CommandParseErrorKind.MissingClosingBracket:
          return "Missing ']' at the end of an array";
        default:
          throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
      }
    }
  }
}