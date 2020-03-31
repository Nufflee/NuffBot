using Discord;

namespace NuffBot.Discord
{
  public class DiscordCommandContext : CommandContext
  {
    public IMessageChannel DiscordChannel { get; }
    
    public DiscordCommandContext(CommandContext context, IMessageChannel discordChannel) 
      : base(context.DiscordGuild, context.TwitchChannel)
    {
      DiscordChannel = discordChannel;
    }
  }
}