using System.Threading.Tasks;

namespace NuffBot.Commands
{
  public class ShowCommand : Command
  {
    public override string Name => "showcmd";
    public override UserLevel UserLevel => UserLevel.God;

    private const string Usage = "Usage: !showcmd <name> - Shows internal representation of a command (currently only support text/database commands - more information: https://github.com/Nufflee/NuffBot/issues/23).";

    protected override async Task Execute<T>(ChatMessage<T> message, CommandContext context, Bot bot)
    {
      CommandParser parser = CommandParser.TryCreate(message.Content);

      if (parser == null)
      {
        bot.SendMessage(Usage, context);

        return;
      }

      string name = parser.ParseArguments()[0];

      DatabaseObject<DatabaseCommand> dbObject = await SqliteDatabase.Instance.ReadSingleAsync<DatabaseCommand>((c) => c.Name == name);

      if (dbObject.Entity == null)
      {
        bot.SendMessage($"Command with name '{name}' doesn't exists!", context);

        return;
      }

      bot.SendMessage($"Command '{dbObject.Entity.Name}' is defined as: `{dbObject.Entity.Name} [{string.Join(", ", dbObject.Entity.Aliases)}] {dbObject.Entity.Response}`", context);
    }
  }
}