using System.Threading.Tasks;
using NuffBot.Core;

namespace NuffBot.Commands
{
  public class AddTimerCommand : Command
  {
    public override string Name => "addtimer";
    public override UserLevel UserLevel => UserLevel.God;

    private const string Usage = "Usage: !addtimer <command> <time> [messages] - Adds a timer for a given command with time delay [minutes] and/or message count trigger.";

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
      int messageTrigger = 0;

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

      DatabaseObject<DatabaseCommand> dbObject = await SqliteDatabase.Instance.ReadSingleAsync<DatabaseCommand>((c) => c.Name == name);

      if (dbObject.Entity == null)
      {
        bot.SendMessage($"Command with name '{name}' doesn't exist!", context);

        return;
      }

      Timer timer = new Timer(dbObject.Entity.Id, timeTrigger, messageTrigger);

      if (!await timer.SaveToDatabase(SqliteDatabase.Instance))
      {
        bot.SendMessage("Failed to add timer to the database.", context);
      }
      else
      {
        bot.SendMessage($"Timer for command '{name}' added successfully!", context);
      }
      
      TwitchBot.TimerManager.ChooseNextTimer();
    }
  }
}