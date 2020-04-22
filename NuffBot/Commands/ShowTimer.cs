using System.Threading.Tasks;
using NuffBot.Core;

namespace NuffBot.Commands
{
  public class ShowTimerCommand : Command
  {
    public override string Name => "showtimer";
    public override UserLevel UserLevel => UserLevel.Viewer;

    private const string Usage = "Usage: !showcmd <name> - Shows internal representation of a timer.";

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

      DatabaseObject<DatabaseCommand> commandDbObject = await SqliteDatabase.Instance.ReadSingleAsync<DatabaseCommand>((c) => c.Name == name);

      if (commandDbObject.Entity == null)
      {
        bot.SendMessage($"Timer '{name}' doesn't exist because there's no such command!", context);

        return;
      }

      DatabaseObject<Timer> timerDbObject = await SqliteDatabase.Instance.ReadSingleAsync<Timer>((t) => t.CommandId == commandDbObject.Entity.Id);

      if (timerDbObject.Entity == null)
      {
        bot.SendMessage($"Timer with name '{name}' doesn't exist!", context);

        return;
      }

      bot.SendMessage($"Timer '{commandDbObject.Entity.Name}' is defined as: `{commandDbObject.Entity.Name} {timerDbObject.Entity.TimeTrigger} {timerDbObject.Entity.MessageTrigger}`", context);
    }
  }
}