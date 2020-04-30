using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NuffBot.Commands
{
  public class ShowAliasesCommand : Command
  {
    public override string Name => "showaliases";
    public override IEnumerable<string> Aliases => new[] {"showalias"};
    public override UserLevel UserLevel => UserLevel.Viewer;

    private const string Usage = "Usage: !showaliases <name> - Shows all aliases of a specified command.";

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

      List<string> aliases = (await DatabaseHelper.GetAllAliasesOfCommand(dbObject.Entity)).Select(a => a.Entity.Alias).ToList();

      if (aliases.Count > 0)
      {
        bot.SendMessage($"Aliases of command '{name}' are: {string.Join(", ", aliases)}", context);
      }
      else
      {
        bot.SendMessage($"Command '{name}' doesn't have any aliases.", context);
      }
    }
  }
}