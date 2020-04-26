using System.Collections.Generic;
using System.Linq;
using System.Timers;
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

    private TimerModel currentTimer;
    private int messageCount;
    private readonly Bot bot;
    private Timer triggerTimer;

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

      List<DatabaseObject<TimerModel>> timers = await DatabaseHelper.GetAllTimers();

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

        triggerTimer = new Timer(currentTimer.TimeTrigger * 1000);

        triggerTimer.Start();

        triggerTimer.Elapsed += (_, __) => ExecuteTimer(currentTimer);
      }
    }

    private async void ExecuteTimer(TimerModel timer)
    {
      DatabaseObject<CommandModel> command = await DatabaseHelper.GetCommandById(timer.CommandId);

      bot.SendMessage(command.Entity.Response, new CommandContext(DiscordBot.CurrentGuild, TwitchBot.CurrentChannel));

      ChooseNextTimer(true);
    }
  }
}