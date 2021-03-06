using System.Threading.Tasks;

namespace NuffBot.Commands
{
  public class UpdateCommand : Command
  {
    public override string Name => "updatecmd";
    public override UserLevel UserLevel => UserLevel.God;

    private const string Usage = "Usage: !updatecmd <name> [aliases[]] <response> - Updates an existing command. If no aliases are specified, it doesn't touch them. Otherwise it overrides them.";

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

      DatabaseObject<CommandModel> dbObject = await DatabaseHelper.GetCommandByNameOrAlias(name);

      if (!dbObject.Exists())
      {
        bot.SendMessage($"Command with name '{name}' doesn't exist!", context);

        return;
      }

      if (aliases != null)
      {
        foreach (string alias in aliases)
        {
          DatabaseObject<AliasModel> dbAlias = await DatabaseHelper.GetAliasByName(alias);

          if (dbAlias.Exists() && dbAlias.Entity.CommandId != dbObject.Entity.Id)
          {
            bot.SendMessage($"Command with name or alias '{alias}' already exists!", context);

            return;
          }
        }

        foreach (DatabaseObject<AliasModel> alias in await DatabaseHelper.GetAllAliasesOfCommand(dbObject.Entity))
        {
          await alias.DeleteFromDatabase(SqliteDatabase.Instance);
        }

        foreach (string alias in aliases)
        {
          if (!await new AliasModel(dbObject.Entity, alias).SaveToDatabase(SqliteDatabase.Instance))
          {
            bot.SendMessage("Failed to save the updated command to the database!", context);

            return;
          }
        }
      }

      if (!string.IsNullOrWhiteSpace(response))
      {
        dbObject.Entity.Response = response;

        if (await dbObject.UpdateInDatabase(SqliteDatabase.Instance))
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
}