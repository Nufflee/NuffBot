using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NuffBot.Commands
{
  public class ShowCommand : Command
  {
    public override string Name => "showcmd";
    public override UserLevel UserLevel => UserLevel.Viewer;

    private const string Usage = "Usage: !showcmd <name> - Shows internal representation of a command (currently only support text/database commands - more information: https://github.com/Nufflee/NuffBot/issues/23).";

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
        bot.SendMessage($"Command with name '{name}' doesn't exist!", context);

        return;
      }

      IEnumerable<string> aliases = (await DatabaseHelper.GetAllAliasesOfCommand(dbObject.Entity)).Select(a => a.Entity.Alias);
      
      bot.SendMessage($"Command '{dbObject.Entity.Name}' is defined as: `{dbObject.Entity.Name} [{string.Join(", ", aliases)}] {dbObject.Entity.Response}`", context);
    }
  }
}