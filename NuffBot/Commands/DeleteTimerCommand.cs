using System.Threading.Tasks;
using NuffBot.Core;

namespace NuffBot.Commands
{
  public class DeleteTimerCommand : Command
  {
    public override string Name => "deltimer";
    public override UserLevel UserLevel => UserLevel.God;

    private const string Usage = "Usage: !deltimer <name> - Deletes a timer from the database.";

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
      
      DatabaseObject<Timer> timerDbObject = await SqliteDatabase.Instance.ReadSingleAsync<Timer>((t) => t.Id == commandDbObject.Entity.Id);

      if (timerDbObject.Entity == null)
      {
        bot.SendMessage($"Timer with name '{name}' doesn't exist!", context);
        
        return;
      }
      
      if (!await timerDbObject.DeleteFromDatabase(SqliteDatabase.Instance))
      {
        bot.SendMessage("Failed to delete timer from the database.", context);
      }
      else
      {
        bot.SendMessage($"Timer '{name}' was deleted successfully!", context);
      }
      
      TwitchBot.TimerManager.ChooseNextTimer(true);
    }
  }
}