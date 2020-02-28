using System.Collections.Generic;
using System.Linq;

namespace NuffBot.Commands
{
  public class CommandParser
  {
    private char CurrentChar => command[currentPosition];
    private string command;
    private int currentPosition;

    private CommandParser()
    {
    }

    public static CommandParser TryCreate(string command)
    {
      command = command.Trim();
      int startPosition = command.IndexOf(' ');

      if (startPosition < 0)
      {
        return null;
      }

      return new CommandParser
      {
        command = command,
        currentPosition = startPosition
      };
    }

    public string ParseWord(params char[] endChars)
    {
      EatWhiteSpace();

      if (IsAtEnd())
      {
        throw new CommandParseError(CommandParseErrorKind.ExpectedWordAtEnd);
      }

      int start = currentPosition;

      while (!char.IsWhiteSpace(CurrentChar) && !endChars.Contains(CurrentChar))
      {
        Advance();

        if (IsAtEnd())
        {
          break;
        }
      }

      return command.Substring(start, currentPosition - start);
    }

    public string[] ParseOptionalArray() => ParseArray(true);
    
    public string[] ParseArray(bool optional = false)
    {
      EatWhiteSpace();

      if (!MatchChar('['))
      {
        if (optional)
        {
          return null;
        }
        
        throw new CommandParseError(CommandParseErrorKind.MissingOpeningBracket);
      }

      EatWhiteSpace();

      List<string> result = new List<string>();
      bool commaPresent = true;

      while (true)
      {
        EatWhiteSpace();

        if (IsAtEnd())
        {
          throw new CommandParseError(CommandParseErrorKind.MissingClosingBracket);
        }

        if (CurrentChar == ',')
        {
          commaPresent = true;

          Advance();
        }

        if (MatchChar(']'))
        {
          break;
        }

        if (!commaPresent)
        {
          throw new CommandParseError(CommandParseErrorKind.MissingComma);
        }

        if (CurrentChar != ',')
        {
          commaPresent = false;
        }

        result.Add(ParseWord(',', ']'));
      }

      return result.ToArray();
    }

    public string ParseRest()
    {
      EatWhiteSpace();

      return command.Substring(currentPosition, command.Length - currentPosition);
    }

    private void EatWhiteSpace()
    {
      if (IsAtEnd())
      {
        return;
      }

      while (char.IsWhiteSpace(CurrentChar))
      {
        Advance();

        if (IsAtEnd())
        {
          break;
        }
      }
    }

    private void Advance()
    {
      currentPosition++;
    }

    private bool MatchChar(char expected)
    {
      if (IsAtEnd())
      {
        return false;
      }
      
      if (CurrentChar == expected)
      {
        Advance();

        return true;
      }

      return false;
    }

    private bool IsAtEnd()
    {
      return currentPosition >= command.Length;
    }
  }
}