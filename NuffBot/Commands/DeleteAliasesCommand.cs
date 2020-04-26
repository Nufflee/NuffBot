using System.Collections.Generic;
using System.Threading.Tasks;

namespace NuffBot.Commands
{
  public class DeleteAliasesCommand : Command
  {
    public override string Name => "delaliases";
    public override IEnumerable<string> Aliases => new[] {"delalias"};
    public override UserLevel UserLevel => UserLevel.God;

    private const string Usage = "Usage: !delaliases <aliases[]> - Deletes specified aliases.";

    protected override async Task Execute<T>(ChatMessage<T> message, CommandContext context, Bot bot)
    {
      CommandParser parser = CommandParser.TryCreate(message.Content);

      if (parser == null)
      {
        bot.SendMessage(Usage, context);

        return;
      }

      string[] aliases;

      try
      {
        aliases = parser.ParseArray();
      }
      catch (CommandParseError error)
      {
        bot.SendMessage(error.Message, context);
        bot.SendMessage(Usage, context);

        return;
      }

      int count = 0;

      foreach (string alias in aliases)
      {
        DatabaseObject<CommandModel> dbObject = await DatabaseHelper.GetCommandByAlias(alias);

        if (dbObject == null)
        {
          bot.SendMessage($"Command with alias '{alias}' doesn't exist!", context);

          continue;
        }

        dbObject.Entity.Aliases.Remove(alias);

        if (await dbObject.UpdateInDatabase(SqliteDatabase.Instance))
        {
          count++;
        }
      }

      if (count == aliases.Length)
      {
        bot.SendMessage($"{count} alias(es) removed successfully!", context);
      }
      else
      {
        bot.SendMessage($"Failed to remove {aliases.Length - count} alias(es)!", context);
      }
    }
  }
}