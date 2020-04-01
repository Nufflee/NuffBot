using System.Collections.Generic;
using System.Threading.Tasks;

namespace NuffBot.Commands
{
  public class AddCommand : Command
  {
    public override string Name => "addcmd";
    public override UserLevel UserLevel => UserLevel.God;

    private const string Usage = "Usage: !addcmd <name> [aliases[]] <response> - Adds a command to the database.";

    protected override async Task Execute<T>(ChatMessage<T> message, CommandContext context, Bot bot)
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

      DatabaseCommand command = new DatabaseCommand(name, aliases, response);
      List<DatabaseObject<DatabaseCommand>> duplicateCommands = await SqliteDatabase.Instance.ReadAllAsync<DatabaseCommand>((dbCommand) => dbCommand.Name == command.Name);
      
      if (duplicateCommands.Count > 0)
      {
        bot.SendMessage($"Command with name '{name}' already exists!", context);

        return;
      }

      if (!await command.SaveToDatabase(SqliteDatabase.Instance))
      {
        bot.SendMessage("Failed to add command to the database.", context);
      }
      else
      {
        bot.SendMessage($"Command '{name}' added successfully!", context);
      }
    }
  }
}