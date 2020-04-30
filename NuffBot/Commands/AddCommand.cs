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

      aliases ??= new string[0];

      if ((await DatabaseHelper.GetCommandByNameOrAlias(name)).Exists())
      {
        bot.SendMessage($"Command with name or alias '{name}' already exists!", context);

        return;
      }

      foreach (string alias in aliases)
      {
        if ((await DatabaseHelper.GetCommandByNameOrAlias(alias)).Exists())
        {
          bot.SendMessage($"Command with name or alias '{alias}' already exists!", context);

          return;
        }
      }

      CommandModel command = new CommandModel(name, response);

      if (!await command.SaveToDatabase(SqliteDatabase.Instance))
      {
        bot.SendMessage("Failed to add command to the database.", context);

        return;
      }

      foreach (string alias in aliases)
      {
        if (!await new AliasModel(command.Id, alias).SaveToDatabase(SqliteDatabase.Instance))
        {
          bot.SendMessage("Failed to add command to the database.", context);

          return;
        }
      }
      
      bot.SendMessage($"Command '{name}' added successfully!", context);
    }
  }
}