using System;
using System.Linq;

namespace NuffBot.Commands
{
  public class UpdateCommand : Command
  {
    public override string Name => "updatecmd";
    public override UserLevel UserLevel => UserLevel.God;

    private const string Usage = "Usage: !updatecmd <name> [aliases[]] <response> - Updates an existing command. If no aliases are specified, it doesn't touch them. Otherwise it overrides them.";

    protected override void Execute<T>(ChatMessage<T> message, CommandContext context, Bot bot)
    {
      CommandParser parser = CommandParser.TryCreate(message.Content);

      if (parser == null)
      {
        bot.SendMessage(Usage, context);

        return;
      }

      string name;
      string[] aliases;
      string response;

      try
      {
        name = parser.ParseWord();
        aliases = parser.ParseOptionalArray();
        response = parser.ParseRest();
      }
      catch (CommandParseError error)
      {
        bot.SendMessage(error.Message, context);
        bot.SendMessage(Usage, context);

        return;
      }

      DatabaseCommand command = SqliteDatabase.Instance.Select<DatabaseCommand>((dbCommand) => dbCommand.Name == name).FirstOrDefault();
      
      if (command == null)
      {
        bot.SendMessage($"Command with name '{name}' doesn't exists!", context);

        return;
      }

      if (aliases != null)
      {
        command.Aliases = aliases;
      }

      command.Response = response;

      if (SqliteDatabase.Instance.Update(command))
      {
        bot.SendMessage($"Command '{name}' updated successfully!", context);
      }
      else
      {
        bot.SendMessage("Failed to save the updated command to the database!", context);
      }
    }
  }
}