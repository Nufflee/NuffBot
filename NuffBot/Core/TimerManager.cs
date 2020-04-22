using System.Collections.Generic;
using System.Linq;
using NuffBot.Commands;
using NuffBot.Discord;

namespace NuffBot.Core
{
  public class TimerManager
  {
    public int MessageCount
    {
      get => messageCount;
      set
      {
        messageCount = value;

        if (currentTimer == null)
        {
          return;
        }
        
        if (currentTimer.MessageTrigger != 0 && messageCount >= currentTimer.MessageTrigger)
        {
          triggerTimer?.Stop();

          ExecuteTimer(currentTimer);
        }
      }
    }

    private Timer currentTimer;
    private int messageCount;
    private readonly Bot bot;
    private System.Timers.Timer triggerTimer;

    public TimerManager(Bot bot)
    {
      this.bot = bot;

      ChooseNextTimer();
    }

    public async void ChooseNextTimer(bool replace = false)
    {
      if (!replace && currentTimer != null)
      {
        return;
      }

      triggerTimer?.Stop();

      List<DatabaseObject<Timer>> timers = await SqliteDatabase.Instance.ReadAllAsync<Timer>();

      // Make sure to not select the same timer twice in a row (if more than 1 is present).
      if (timers.Count > 1)
      {
        timers = timers.Where((t) => t.Entity.Id != currentTimer.Id).ToList();
      }

      currentTimer = timers.Random()?.Entity;

      if (currentTimer != null)
      {
        if (currentTimer.MessageTrigger != 0)
        {
          currentTimer.MessageTrigger += messageCount;
        }
        
        if (currentTimer.TimeTrigger == 0)
        {
          return;
        }

        triggerTimer = new System.Timers.Timer(currentTimer.TimeTrigger * 1000);

        triggerTimer.Start();

        triggerTimer.Elapsed += (_, __) => ExecuteTimer(currentTimer);
      }
    }

    private async void ExecuteTimer(Timer timer)
    {
      DatabaseObject<DatabaseCommand> command = await SqliteDatabase.Instance.ReadSingleAsync<DatabaseCommand>((c) => c.Id == timer.CommandId);

      bot.SendMessage(command.Entity.Response, new CommandContext(DiscordBot.CurrentGuild, TwitchBot.CurrentChannel));

      ChooseNextTimer(true);
    }
  }
}