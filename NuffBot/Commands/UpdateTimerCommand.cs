using System.Threading.Tasks;
using NuffBot.Core;

namespace NuffBot.Commands
{
  public class UpdateTimerCommand : Command
  {
    public override string Name => "updatetimer";
    public override UserLevel UserLevel => UserLevel.God;

    private const string Usage = "Usage: !updatetimer <name> <time> [messages] - Updates an existing timer.";

    protected override async Task Execute<T>(ChatMessage<T> message, CommandContext context, Bot bot)
    {
      CommandParser parser = CommandParser.TryCreate(message.Content);

      if (parser == null)
      {
        bot.SendMessage(Usage, context);

        return;
      }

      string name;
      int timeTrigger;
      int messageTrigger = -1;

      try
      {
        name = parser.ParseWord();

        if (!int.TryParse(parser.ParseWord(), out timeTrigger))
        {
          bot.SendMessage("'time' parameter is not an int!", context);

          return;
        }

        string messageTriggerString = parser.ParseOptionalWord();

        if (messageTriggerString != null && !int.TryParse(messageTriggerString, out messageTrigger))
        {
          bot.SendMessage("'messages' parameter is not an int!", context);

          return;
        }
      }
      catch (CommandParseError error)
      {
        bot.SendMessage(error.Message, context);
        bot.SendMessage(Usage, context);

        return;
      }

      if (timeTrigger == 0 && messageTrigger == 0)
      {
        bot.SendMessage("'time' and 'messages' parameters can't both be 0.", context);

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

      timerDbObject.Entity.TimeTrigger = timeTrigger;

      if (messageTrigger != -1)
      {
        timerDbObject.Entity.MessageTrigger = messageTrigger;
      }

      if (!await timerDbObject.UpdateInDatabase(SqliteDatabase.Instance))
      {
        bot.SendMessage("Failed to update timer from the database.", context);
      }
      else
      {
        bot.SendMessage($"Timer '{name}' was updated successfully!", context);
      }

      TwitchBot.TimerManager.ChooseNextTimer(true);
    }
  }
}