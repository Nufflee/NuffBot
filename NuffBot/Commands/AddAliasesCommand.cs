using System.Linq;
using System.Threading.Tasks;

namespace NuffBot.Commands
{
  public class AddAliasesCommand : Command
  {
    public override string Name => "addaliases";
    public override UserLevel UserLevel => UserLevel.God;

    private const string Usage = "Usage: !addaliases <name> <aliases[]> - Adds new alises to the specified command.";

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

      try
      {
        name = parser.ParseWord();
        aliases = parser.ParseArray();
      }
      catch (CommandParseError error)
      {
        bot.SendMessage(error.Message, context);
        bot.SendMessage(Usage, context);

        return;
      }

      DatabaseObject<DatabaseCommand> dbObject = await SqliteDatabase.Instance.ReadSingleAsync<DatabaseCommand>((dbCommand) => dbCommand.Name == name);

      if (dbObject.Entity == null)
      {
        bot.SendMessage($"Command with name '{name}' doesn't exists!", context);

        return;
      }

      dbObject.Entity.Aliases = dbObject.Entity.Aliases.Concat(aliases).ToArray();

      if (await dbObject.UpdateInDatabase(SqliteDatabase.Instance))
      {
        bot.SendMessage("Alias(es) added successfully!", context);
      }
      else
      {
        bot.SendMessage("Failed to save new alias(es)!", context);
      }
    }
  }
}