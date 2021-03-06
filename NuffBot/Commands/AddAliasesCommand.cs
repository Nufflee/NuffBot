using System.Collections.Generic;
using System.Threading.Tasks;

namespace NuffBot.Commands
{
  public class AddAliasesCommand : Command
  {
    public override string Name => "addaliases";
    public override IEnumerable<string> Aliases => new[] {"addalias"};
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

      DatabaseObject<CommandModel> dbObject = await DatabaseHelper.GetCommandByNameOrAlias(name);

      if (!dbObject.Exists())
      {
        bot.SendMessage($"Command or alias with name '{name}' doesn't exist!", context);

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

      foreach (string alias in aliases)
      {
        if (!await new AliasModel(dbObject.Entity, alias).SaveToDatabase(SqliteDatabase.Instance))
        {
          bot.SendMessage("Failed to add command to the database.", context);

          return;
        }
      }
      
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