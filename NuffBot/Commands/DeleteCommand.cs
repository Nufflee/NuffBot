using System.Threading.Tasks;

namespace NuffBot.Commands
{
  public class DeleteCommand : Command
  {
    public override string Name => "delcmd";
    public override UserLevel UserLevel => UserLevel.God;

    private const string Usage = "Usage: !delcmd <name> - Deletes a command from the database.";

    protected override async Task Execute<T>(ChatMessage<T> message, CommandContext context, Bot bot)
    {
      CommandParser parser = CommandParser.TryCreate(message.Content);

      if (parser == null)
      {
        bot.SendMessage(Usage, context);

        return;
      }

      string name;

      try
      {
        name = parser.ParseWord();
      }
      catch (CommandParseError error)
      {
        bot.SendMessage(error.Message, context);
        bot.SendMessage(Usage, context);

        return;
      }

      DatabaseObject<CommandModel> dbObject = await DatabaseHelper.GetCommandByNameOrAlias(name);

      if (!dbObject.Exists())
      {
        bot.SendMessage($"Command with name or alias '{name}' doesn't exist!", context);

        return;
      }

      if (!await dbObject.DeleteFromDatabase(SqliteDatabase.Instance))
      {
        bot.SendMessage("Failed to delete command from the database.", context);
      }
      else
      {
        bot.SendMessage($"Command '{name}' deleted successfully!", context);
      }
    }
  }
}