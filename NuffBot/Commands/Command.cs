using System.Collections.Generic;
using System.Threading.Tasks;
using NuffBot.Core;
using NuffBot.Core.Twitch;
using NuffBot.Discord;

namespace NuffBot.Commands
{
  public abstract class Command
  {
    public abstract string Name { get; }
    public virtual UserLevel UserLevel => UserLevel.Viewer;
    public virtual IEnumerable<string> Aliases => new string[0];

    public void DoExecute<T>(ChatMessage<T> message, Bot bot, CommandContext context)
      where T : User
    {
      Execute(message, context, bot);

      switch (message)
      {
        case DiscordChatMessage discordMessage:
          ExecuteDiscord(discordMessage, (DiscordCommandContext) context, (DiscordBot) bot);
          break;
        case TwitchChatMessage twitchMessage:
          ExecuteTwitch(twitchMessage, context, (TwitchBot) bot);
          break;
      }
    }

    protected virtual Task Execute<T>(ChatMessage<T> message, CommandContext context, Bot bot)
      where T : User
    {
      return Task.CompletedTask;
    }

    protected virtual void ExecuteTwitch(TwitchChatMessage message, CommandContext context, TwitchBot bot)
    {
    }

    protected virtual void ExecuteDiscord(DiscordChatMessage message, DiscordCommandContext context, DiscordBot bot)
    {
    }
  }
}