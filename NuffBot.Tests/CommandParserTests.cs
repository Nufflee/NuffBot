using NuffBot.Commands;
using Xunit;

namespace NuffBot.Tests
{
  public class CommandParserTests
  {
    [Theory]
    [InlineData("!command there is a bunch of words here", "there")]
    [InlineData("!command there", "there")]
    [InlineData("!command                there               are big spaces here", "there")]
    public void TestParseWord(string text, string expectedWord)
    {
      CommandParser parser = CommandParser.TryCreate(text);

      Assert.Equal(expectedWord, parser.ParseWord());
    }

    [Theory]
    [InlineData("!command")]
    [InlineData("!command                       ")]
    [InlineData("!command           here are words", false)]
    public void TestParserCreation(string text, bool expectedNull = true)
    {
      CommandParser parser = CommandParser.TryCreate(text);

      if (expectedNull)
      {
        Assert.Null(parser);
      }
      else
      {
        Assert.NotNull(parser);
      }
    }

    [Theory]
    [InlineData("!command word", CommandParseErrorKind.ExpectedWordAtEnd)]
    public void TestParseWordErrors(string text, CommandParseErrorKind expectedKind)
    {
      CommandParser parser = CommandParser.TryCreate(text);

      parser.ParseWord();

      CommandParseError error = Assert.Throws<CommandParseError>(() => parser.ParseWord());

      Assert.Equal(expectedKind, error.Kind);
    }

    [Theory]
    [InlineData("!command [a, b, c, d, e, f, g]", new[] {"a", "b", "c", "d", "e", "f", "g"})]
    [InlineData("!command      [     a   , b , c ,   d,   e, f, g     ]     ", new[] {"a", "b", "c", "d", "e", "f", "g"})]
    [InlineData("!command [a]", new[] {"a"})]
    [InlineData("!command []", new string[0])]
    public void TestParseArray(string text, string[] expectedArray)
    {
      CommandParser parser = CommandParser.TryCreate(text);

      Assert.Equal(expectedArray, parser.ParseArray());
    }

    [Theory]
    [InlineData("!command a, b, c]", CommandParseErrorKind.MissingOpeningBracket)]
    [InlineData("!command a", CommandParseErrorKind.MissingOpeningBracket)]
    [InlineData("!command [", CommandParseErrorKind.MissingClosingBracket)]
    [InlineData("!command [a, b, c", CommandParseErrorKind.MissingClosingBracket)]
    [InlineData("!command [a b c]", CommandParseErrorKind.MissingComma)]
    [InlineData("!command [a, b c,]", CommandParseErrorKind.MissingComma)]
    public void TestParseArrayErrors(string text, CommandParseErrorKind expectedKind)
    {
      CommandParser parser = CommandParser.TryCreate(text);

      CommandParseError error = Assert.Throws<CommandParseError>(() => parser.ParseArray());

      Assert.Equal(expectedKind, error.Kind);
    }

    
    [Theory]
    [InlineData("!command this is text]")]
    [InlineData("!command this also is text]")]
    [InlineData("!command , sdsa ]")]
    [InlineData("!command [a, b, c, d, e, f, g]", new[] {"a", "b", "c", "d", "e", "f", "g"})]
    public void TestParseOptionalArray(string text, string[] expectedArray = null)
    {
      CommandParser parser = CommandParser.TryCreate(text);

      string[] array = parser.ParseOptionalArray();

      Assert.Equal(expectedArray, array);
    }
  }
}